using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MnistImageStore.Models;
using MnistImageStore.Persistence;
using TryML.Utilities.ExceptionHandling;

namespace MnistImageStore
{
    public class Startup
    {
        protected const string applicationSettingsFileNane = "appsettings.json";
        protected const string mnistImageStoreSettingsKeyName = "MnistImageStore";
        protected const string jsonHttpContentType = "application/json";

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            
            var builder = new ConfigurationBuilder()
                .SetBasePath(hostEnvironment.ContentRootPath)
                .AddJsonFile(applicationSettingsFileNane, optional: false, reloadOnChange: false);

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.AddSingleton(typeof(ExceptionToHttpStatusCodeConverter), new ExceptionToHttpStatusCodeConverter());
            services.AddSingleton(typeof(ExceptionToHttpInternalServerErrorConverter), new ExceptionToHttpInternalServerErrorConverter());
            SetupMnistImageDataStructures(services);

            services.AddControllers().AddNewtonsoftJson();
            services.AddMvc(options =>
            {
                options.RespectBrowserAcceptHeader = true;
                options.ReturnHttpNotAcceptable = true;  // Return HTTP 406
            }); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            MnistImageStoreOptions options = Configuration.GetSection(mnistImageStoreSettingsKeyName).Get<MnistImageStoreOptions>();
            if (options.ShowDeveloperExceptionPage == true)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                SetupExceptionHandler(app);
            }

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// Sets up a custom exception handler in the application's pipeline.
        /// </summary>
        /// <param name="app">A class which allows configuration of the application's request pipeline.</param>
        protected void SetupExceptionHandler(IApplicationBuilder app)
        {
            // As per https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-5.0#exception-handler-lambda
            app.UseExceptionHandler(errorApp =>
            {
                var exceptionToHttpStatusCodeConverter = (ExceptionToHttpStatusCodeConverter)app.ApplicationServices.GetService(typeof(ExceptionToHttpStatusCodeConverter));
                var exceptionToHttpInternalServerErrorConverter = (ExceptionToHttpInternalServerErrorConverter)app.ApplicationServices.GetService(typeof(ExceptionToHttpInternalServerErrorConverter));

                errorApp.Run(async context =>
                {
                    // Get the exception
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    Exception exception = exceptionHandlerPathFeature.Error;

                    if (exception != null)
                    {
                        context.Response.ContentType = jsonHttpContentType;
                        context.Response.StatusCode = (Int32)exceptionToHttpStatusCodeConverter.Convert(exception);
                        HttpInternalServerError httpInternalServerError = exceptionToHttpInternalServerErrorConverter.Convert(exception);
                        var serializer = new HttpInternalServerErrorJsonSerializer();
                        await context.Response.WriteAsync(serializer.Serialize(httpInternalServerError).ToString());
                    }
                    else
                    {
                        // Not sure if this situation can arise, but will leave this handler in while testing
                        throw new Exception("'exceptionHandlerPathFeature.Error' was null whilst handling exception.");
                    }
                });
            });
        }

        /// <summary>
        /// Sets up objects storing and indexing MNIST image data, and adds them to the specified services collection.
        /// </summary>
        /// <param name="services">The services collection to add the objects to.</param>
        protected void SetupMnistImageDataStructures(IServiceCollection services)
        {
            MnistImageStoreOptions options = Configuration.GetSection(mnistImageStoreSettingsKeyName).Get<MnistImageStoreOptions>();
            // Read the MNIST data
            var mnistImageReader = new MnistImageNativeFormatFileReader(options.MnistImageDataFileUri, options.MnistLabelFileUri);
            List<MnistImage> allMnistImages = null;
            // This stores the images grouped by label. 
            MnistImageLabelIndex mnistImagesByLabel = null;
            try
            {
                IEnumerable<MnistImage> allImages = mnistImageReader.Read();
                allMnistImages = new List<MnistImage>(allImages);
                allImages = mnistImageReader.Read();
                mnistImagesByLabel = new MnistImageLabelIndex(allMnistImages);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to read MNIST images.", e);
            }

            services.AddSingleton<List<MnistImage>>(allMnistImages);
            services.AddSingleton<MnistImageLabelIndex>(mnistImagesByLabel);
        }
    }
}
