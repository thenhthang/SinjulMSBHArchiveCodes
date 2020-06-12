using System.Linq;
using System.Threading.Tasks;

using JsonLogger.SinjulMSBH;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JsonLogger
{
    public class Program
    {
        public static Task Main(string[] args) => CreateHostBuilder(args).Build().RunAsync();

        //? In ASP.NET Core 2
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((host, builder) =>
                {
                    // Register json logger in non-development environments only.
                    if (!host.HostingEnvironment.IsDevelopment())
                    {
                        // Remove default loggers
                        foreach (var s in builder.Services.Where(service => service.ServiceType == typeof(ILoggerProvider)).ToList())
                        {
                            builder.Services.Remove(s);
                        }

                        builder.Services.AddSingleton<ILoggerProvider, JsonLoggerProvider>();
                    }
                })
                .UseStartup<Startup>();


        //? In .NET Core 3.0
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging((host, builder) =>
                {
                    // Register json logger in non-development environments only.
                    if (!host.HostingEnvironment.IsDevelopment())
                    {
                        // Remove default loggers
                        foreach (var s in builder.Services.Where(service => service.ServiceType == typeof(ILoggerProvider)).ToList())
                        {
                            builder.Services.Remove(s);
                        }

                        builder.Services.AddSingleton<ILoggerProvider, JsonLoggerProvider>();
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseKestrel(options => options.AddServerHeader = false);
                });
    }
}
