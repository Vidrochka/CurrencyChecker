using System;
using System.IO;
using System.Threading.Tasks;
using CurrencyChecker.Utils;
using Xunit;

namespace CurrencyChecker.Test
{
    public class RequestTest
    {
        [Fact]
        public async Task GetRequest_Ok_Test()
        {
            WebWorker web = new WebWorker(SU.GetConfiguration(), SU.GetLogger<WebWorker>());

            await using Stream response = await web.RequestNewCurrencyAsync(DateTime.Today);

            Assert.NotNull(response);

            using StreamReader responseReader = new StreamReader(response);
            string stringResponse = await responseReader.ReadToEndAsync();

            Assert.True(stringResponse.Length>0);
        }
    }
}