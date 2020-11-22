using System;
using System.Threading.Tasks;
using CurrencyChecker.Service;
using CurrencyChecker.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Hosting;
using NLog.Extensions.Logging;

namespace CurrencyChecker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration(configBuilder =>
                {
                    configBuilder.AddJsonFile("config.json");
                    configBuilder.AddCommandLine(args);
                })
                .ConfigureLogging(loggerConfiguration =>
                {
#if DEBUG
                    loggerConfiguration.AddDebug();
#endif
                    loggerConfiguration.AddNLog("NLog.config");
                })
                .UseNLog()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<DbWorker>();
                    services.AddSingleton<WebWorker>();
                    services.AddSingleton<ResponseParser>();
                    services.AddSingleton<ExchangeRatesDdContext>();

                    services.AddHostedService<ServiceShell>();

                    if (!Environment.UserInteractive)
                    {
                        services.AddSingleton<IHostLifetime, ServiceBaseLifetime>();
                    }
                });

            if (Environment.UserInteractive)
            {
                await builder.RunConsoleAsync();
            }
            else
            {
                await builder.Build().RunAsync();
            }
            
        }
    }
}
