using Microsoft.EntityFrameworkCore;
using SharedModels;

namespace ExchangeRateSelector
{
    public class ExchangeRatesDdContext : DbContext
    {
        public DbSet<ExchangeRates> ExchangeRates { get; set; }

        public ExchangeRatesDdContext(DbContextOptions<ExchangeRatesDdContext> options)
            : base(options)
        {
            Database.EnsureCreated();

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