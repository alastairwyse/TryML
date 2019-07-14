using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace TryML
{
    public class Startup
    {
        private readonly String defaultCorsPolicyName = "DefaultCorsPolicy";
        private Action<CorsPolicyBuilder> configureCorsPolicyAction;

        public Startup(IConfiguration configuration)
        {
            configureCorsPolicyAction = new Action<CorsPolicyBuilder>(builder => 
            {
                builder.WithOrigins("http://localhost:4200");
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
            });

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(defaultCorsPolicyName, configureCorsPolicyAction);
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(defaultCorsPolicyName);
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
