using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace CurrencyChecker.Test
{
    public static class SU
    {
        public static IConfiguration GetConfiguration(string configName = @".\TestData\testConfig.json")
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(configName)
                .AddEnvironmentVariables();

            return builder.Build();
        }

        public static ILogger<T> GetLogger<T>()
        {
            return Mock.Of<ILogger<T>>();
        }

        public static FileStream GetFileStream(string fileName)
        {
            return File.Open(fileName, FileMode.Open, FileAccess.ReadWrite);
        }
    }
}