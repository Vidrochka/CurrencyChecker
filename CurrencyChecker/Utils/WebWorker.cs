using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace CurrencyChecker.Utils
{
    /// <summary>
    /// Сервис по работе с веб запросами
    /// </summary>
    public class WebWorker
    {
        private readonly ILogger<WebWorker> _logger;
        private readonly IConfiguration _configuration;

        public WebWorker(IConfiguration configuration, ILogger<WebWorker> logger)
        {
            _configuration = configuration ?? throw new NullReferenceException($"{nameof(configuration)} is null");
            _logger = logger ?? throw new NullReferenceException($"{nameof(logger)} is null");
        }

        /// <summary>
        /// Делает запрос курса за дату
        /// </summary>
        /// <param name="date">Дата за которую проводится запрос</param>
        /// <returns></returns>
        public async Task<Stream> RequestNewCurrencyAsync(DateTime date)
        {
            string requestAddress = string.Empty;

            try
            {
                _logger.LogInformation($"{nameof(WebWorker)} start");
                requestAddress = string.Format(_configuration.GetRequestUri(), date.ToString("dd.MM.yyyy"));

                _logger.LogInformation($"Request address - [{requestAddress}]");

                HttpWebRequest request = WebRequest.CreateHttp(requestAddress);
                request.Method = "GET";
                request.Timeout = _configuration.GetWebRequestTimeout();
                request.AllowAutoRedirect = false;

                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync().ConfigureAwait(false);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Request fail [{(int)response.StatusCode}]");
                }

                Stream responseStream = response.GetResponseStream();

                _logger.LogInformation($"{nameof(WebWorker)} done");

                return responseStream;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Request was fail. Address format - [{_configuration.GetRequestUri()}] | Address - [{requestAddress}] | Error : {ex}");
                return null;
            }
        }
    }
}