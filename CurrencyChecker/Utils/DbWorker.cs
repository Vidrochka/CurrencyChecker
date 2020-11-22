using System;
using System.Linq;
using System.Threading.Tasks;
using CurrencyChecker.DTO;
using CurrencyChecker.Exceptions;
using SharedModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CurrencyChecker.Utils
{
    /// <summary>
    /// Сервис по работе с базой данных
    /// </summary>
    public class DbWorker
    {
        private readonly ILogger<DbWorker> _logger;
        private readonly IConfiguration _configuration;
        private readonly ExchangeRatesDdContext _dbContext;

        public DbWorker(IConfiguration configuration, ILogger<DbWorker> logger, ExchangeRatesDdContext dbContext)
        {
            _configuration = configuration ?? throw new NullReferenceException($"{nameof(configuration)} is null");
            _logger = logger ?? throw new NullReferenceException($"{nameof(logger)} is null");
            _dbContext = dbContext ?? throw new NullReferenceException($"{nameof(dbContext)} is null");
        }

        /// <summary>
        /// Сохраняет курсы валют в базу данных
        /// </summary>
        /// <param name="curs">Курсы валют который неободимо сохранить в базу</param>
        /// <returns></returns>
        public async Task<bool> StoreCurrencyAsync(ValCurs curs)
        {
            try
            {
                _logger.LogInformation($"{nameof(DbWorker)} start");

                if (curs is null)
                    throw  new NullReferenceException($"{nameof(curs)} is null");

                if (_dbContext.ExchangeRates.Where(valute => valute.ExchangeRateDate == curs.Date).Any())
                    throw new ExchangeRatesSaveException($"Valute already save today. We dont know how to save new value");

                curs.Valute.ForEach(valute
                    => _dbContext.ExchangeRates.AddAsync( new ExchangeRates()
                    {
                        ExchangeRateDate = curs.Date,
                        Name = valute.Name,
                        CharCode = valute.CharCode,
                        Nominal = valute.Nominal,
                        Value = valute.Value
                    }));

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"{nameof(DbWorker)} done");
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Cant store in db: {ex}");
                return false;
            }
        }
    }
}