using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MnistImageStore.Models;
using MnistImageStore.Persistence;

using System.IO;

namespace MnistImageStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(new Action<IWebHostBuilder>
            ( 
                (webBuilder) =>
                {
                    webBuilder.ConfigureServices(services =>
                    {
                    });
                    webBuilder.UseStartup<Startup>();
                })
            );

            return hostBuilder;
        }
    }
}
