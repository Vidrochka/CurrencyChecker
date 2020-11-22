using System.IO;
using System.Text;
using System.Threading.Tasks;
using CurrencyChecker.DTO;
using CurrencyChecker.Utils;
using Microsoft.Extensions.Logging;
using Xunit;

namespace CurrencyChecker.Test
{
    public class ParserTest
    {
        [Fact]
        public async Task ParseResponse_Ok_Test()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            ILogger<ResponseParser> logger = SU.GetLogger<ResponseParser>();

            ResponseParser parser = new ResponseParser(logger);

            await using FileStream testResponse = SU.GetFileStream(@".\TestData\ResponseOk.xml");

            ValCurs curs = await parser.ParseCursAsync(testResponse);

            Assert.Equal(2, curs.Valute.Count);
        }

        [Fact]
        public async Task ParseResponse_Bad_Test()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            ILogger<ResponseParser> logger = SU.GetLogger<ResponseParser>();

            ResponseParser parser = new ResponseParser(logger);

            await using FileStream testResponse = SU.GetFileStream(@".\TestData\ResponseBad.xml");

            ValCurs curs = await parser.ParseCursAsync(testResponse);

            Assert.Null(curs);
        }
    }
}