using System;
using System.Threading.Tasks;
using CurrencyChecker.Service;
using CurrencyChecker.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
                    loggerConfiguration.AddConsole();
#if DEBUG
                    loggerConfiguration.AddDebug();
#endif
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<DbWorker>();
                    services.AddSingleton<WebWorker>();
                    services.AddSingleton<ResponseParser>();
                    services.AddSingleton<ExchangeRatesDdContext>();

                    services.AddHostedService<ServiceShell>();
                });

            await builder.RunConsoleAsync();
        }
    }
}
