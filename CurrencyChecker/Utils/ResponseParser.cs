using CurrencyChecker.DTO;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CurrencyChecker.Utils
{
    /// <summary>
    /// Парсит веб ответы
    /// </summary>
    public class ResponseParser
    {
        private readonly ILogger<ResponseParser> _logger;

        public ResponseParser(ILogger<ResponseParser> logger)
        {
            _logger = logger ?? throw new NullReferenceException($"{nameof(logger)} is null");

            // В .net core из коробки нет поддержки win1251, необходимо подключить ручками
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        /// Десериализует Stream с xml ответом
        /// </summary>
        /// <param name="responseData">Разбираемые даннные в XML формате</param>
        /// <returns></returns>
        public async Task<ValCurs> ParseCursAsync(Stream responseData)
        {
            if (responseData is null)
                throw new NullReferenceException($"{nameof(responseData)} is null");

            try
            {
                _logger.LogInformation($"{nameof(ResponseParser)} start");

                XmlSerializer formatter = new XmlSerializer(typeof(ValCurs));

                using StreamReader responseReader = new StreamReader(responseData, Encoding.GetEncoding(1251));
                string response = await responseReader.ReadToEndAsync().ConfigureAwait(false);

#if DEBUG
                _logger.LogInformation(response);
#endif

                // Можно сразу читать стрим, но тогда не выйдет логгировать ответ. Стрим не поддерживает Seek
                StringReader responseStringReader = new StringReader(response);
                ValCurs curs = (ValCurs)formatter.Deserialize(responseStringReader);

                LogCurs(curs);

                _logger.LogInformation($"{nameof(ResponseParser)} done");

                return curs;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Cant parse data - {ex}");
                return null;
            }
        }

        private void LogCurs(ValCurs curs)
        {
            _logger.LogInformation($"Response has:{Environment.NewLine}" +
                                   $"date - [{curs.Date}]{Environment.NewLine}" +
                                   $"name - [{curs.Name}]{Environment.NewLine}" +
                                   $"row count - [{curs?.Valute?.Count() ?? -1}]");
        }
    }
}