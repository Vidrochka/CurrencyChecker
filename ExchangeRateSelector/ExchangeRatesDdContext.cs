using Microsoft.EntityFrameworkCore;
using NLog;
using SharedModels;
using System;

namespace ExchangeRateSelector
{
    public class ExchangeRatesDdContext : DbContext
    {
        public DbSet<ExchangeRates> ExchangeRates { get; set; }

        private readonly ILogger _logger;

        public readonly string ConnectionString;

        public ExchangeRatesDdContext(ILogger logger, string connectionString)
        {
            _logger = logger ?? throw new NullReferenceException($"{nameof(logger)} in null");
            ConnectionString = connectionString;
            Database.EnsureCreated();

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
#if DEBUG
            optionsBuilder.LogTo((message) => _logger.Info(message));
#endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExchangeRates>()
                .HasIndex(valute => new { valute.CharCode, valute.ExchangeRateDate });

            modelBuilder.Entity<ExchangeRates>().Property(valute => valute.Value).HasPrecision(18, 4);
            modelBuilder.Entity<ExchangeRates>()
                .HasKey(valute => new { valute.ExchangeRateDate, valute.CharCode });
        }
    }
}