using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace weatherio
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    HostConfig.CertPath = context.Configuration["CertPath"];//from JSON Files
                    HostConfig.CertPasswoord = context.Configuration["CertPassword"];//from Secret Manager storage, in user profile app data 
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        //serverOptions.UseHttps(HostConfig.CertPath, HostConfig.CertPasswoord);
                        serverOptions.ListenAnyIP(5200);
                        serverOptions.ListenAnyIP(5201, listenOptions =>
                        {
                            listenOptions.UseHttps(HostConfig.CertPath, HostConfig.CertPasswoord);
                        });
                    });

                    webBuilder.UseStartup<Startup>();
                });
    }

    public static class HostConfig
    {
        public static string CertPath { get; set; }
        public static string CertPasswoord { get; set; }

    }
}
