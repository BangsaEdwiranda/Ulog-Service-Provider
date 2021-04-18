using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MiddlewareIdentityProvider
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureLogging(configureLogging => configureLogging.ClearProviders());
                    webBuilder.UseSerilog((webHostBuilderContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(webHostBuilderContext.Configuration));
                    webBuilder.UseStartup<Startup>();
                });
    }
}
