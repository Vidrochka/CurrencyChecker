using System;
using SharedModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CurrencyChecker.Utils
{
    public class ExchangeRatesDdContext : DbContext
    {
        public DbSet<ExchangeRates> ExchangeRates { get; set; }

        private readonly IConfiguration _configuration;
        private readonly ILogger<ExchangeRatesDdContext> _logger;

        public ExchangeRatesDdContext(IConfiguration configuration, ILogger<ExchangeRatesDdContext> logger)
        {
            _configuration = configuration ?? throw new NullReferenceException($"{nameof(configuration)} in null");
            _logger = logger ?? throw new NullReferenceException($"{nameof(logger)} in null");
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetDdConnectionString());
#if DEBUG
            optionsBuilder.LogTo((message)=>_logger.LogInformation(message));
#endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExchangeRates>()
                .HasIndex(valute => new {valute.CharCode, valute.ExchangeRateDate});

            modelBuilder.Entity<ExchangeRates>().Property(valute => valute.Value).HasPrecision(18, 4);
            modelBuilder.Entity<ExchangeRates>()
                .HasKey(valute => new {valute.ExchangeRateDate, valute.CharCode});
        }
    }
}