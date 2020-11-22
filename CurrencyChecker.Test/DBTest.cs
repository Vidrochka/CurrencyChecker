using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CurrencyChecker.DTO;
using Xunit;
using CurrencyChecker.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Moq;
using SharedModels;

namespace CurrencyChecker.Test
{
    public class DBTests
    {
        [Fact]
        public async Task DataBaseSaveCurs_Ok_Test()
        {
            IConfiguration config = SU.GetConfiguration();

            ExchangeRatesDdContext dbContext = new ExchangeRatesDdContext(config, SU.GetLogger<ExchangeRatesDdContext>());
            DbWorker db = new DbWorker(config, SU.GetLogger<DbWorker>(), dbContext);

            Valute saveValute = new Valute()
            {
                CharCode = "TEST_1",
                Name = "TEST_1",
                Nominal = 10,
                Value = 10.1m
            };

            bool result = await db.StoreCurrencyAsync(new ValCurs()
            {
                Date = DateTime.Today.AddYears(-100), Name = "TEST_0", Valute = new List<Valute>()
                {
                    saveValute
                }
            });

            Assert.True(result);

            IQueryable<ExchangeRates> restoreValute = dbContext.ExchangeRates.Where(valute => valute.Name == saveValute.Name &&
                                                    valute.Value == saveValute.Value &&
                                                    saveValute.Nominal == saveValute.Nominal);

            Assert.True(restoreValute.Any());

            dbContext.ExchangeRates.Remove(restoreValute.Single());
            dbContext.SaveChanges();
        }

        [Fact]
        public async Task DataBaseSaveCursTwoTime_Bad_Test()
        {
            IConfiguration config = SU.GetConfiguration();

            ExchangeRatesDdContext dbContext = new ExchangeRatesDdContext(config, SU.GetLogger<ExchangeRatesDdContext>());
            DbWorker db = new DbWorker(config, SU.GetLogger<DbWorker>(), dbContext);

            Valute saveValute = new Valute()
            {
                CharCode = "TEST_2",
                Name = "TEST_2",
                Nominal = 10,
                Value = 10.1m
            };

            bool result = await db.StoreCurrencyAsync(new ValCurs()
            {
                Date = DateTime.Today.AddYears(-100),
                Name = "TEST_0",
                Valute = new List<Valute>()
                {
                    saveValute
                }
            });

            Assert.True(result);

            result = await db.StoreCurrencyAsync(new ValCurs()
            {
                Date = DateTime.Today.AddYears(-100),
                Name = "TEST_0",
                Valute = new List<Valute>()
                {
                    saveValute
                }
            });

            Assert.False(result);

            IQueryable<ExchangeRates> restoreValute 
                = dbContext.ExchangeRates.Where(valute => valute.Name == saveValute.Name && 
                                                          valute.Value == saveValute.Value && 
                                                          saveValute.Nominal == saveValute.Nominal);

            Assert.True(restoreValute.Any());

            dbContext.ExchangeRates.Remove(restoreValute.Single());
            dbContext.SaveChanges();
        }
    }
}
