using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MnistImageStore.Models;
using MnistImageStore.Persistence;

namespace MnistImageStore
{
    public class Startup
    {
        protected const string applicationSettingsFileNane = "appsettings.json";
        protected const string mnistImageStoreSettingsKeyName = "MnistImageStore";

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

            SetupMnistImageDataStructures(services);

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// Sets up objects storing and indexing MNIST image data, and adds them to the specified services collection,
        /// </summary>
        /// <param name="services">The services collection to add the objects to.</param>
        protected void SetupMnistImageDataStructures(IServiceCollection services)
        {
            MnistImageStoreOptions options = Configuration.GetSection(mnistImageStoreSettingsKeyName).Get<MnistImageStoreOptions>();
            // Read the MNIST data
            var mnistImageReader = new MnistImageNativeFormatFileReader(options.MnistImageDataFileUri, options.MnistLabelFileUri);
            var allMnistImages = new List<MnistImage>();
            // This stores the images grouped by label.  All images of the same label are within one key of the dictionary.  The dictionary value holds references to the images objects in variable 'allMnistImages'...
            //   i.e. the value in the list is the index of the element in 'allMnistImages'.
            var mnistImagesByLabel = new Dictionary<Int32, List<Int32>>();
            try
            {
                foreach (MnistImage currentImage in mnistImageReader.Read())
                {
                    if (currentImage.Label < 0 || currentImage.Label > 9)
                        throw new Exception("Out of range MINST label encountered.  'Label' property must be between 0 and 9 inclusive.");
                    allMnistImages.Add(currentImage);
                    if (mnistImagesByLabel.ContainsKey(currentImage.Label) == false)
                    {
                        mnistImagesByLabel.Add(currentImage.Label, new List<Int32>());
                    }
                    mnistImagesByLabel[currentImage.Label].Add(allMnistImages.Count - 1);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to read MNIST images.", e);
            }

            // TODO: Think about making proper classes for these data structures... since adding generic Lists and Dictionaries to DI precludes other uses of the same types
            services.AddSingleton<List<MnistImage>>(allMnistImages);
            services.AddSingleton<Dictionary<Int32, List<Int32>>>(mnistImagesByLabel);
        }
    }
}
