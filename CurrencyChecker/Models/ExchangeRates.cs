using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace CurrencyChecker.Models
{
    [Serializable]
    public class ExchangeRates
    {
        [Required]
        public DateTime ExchangeRateDate { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string CharCode { get; set; }
        [Required]
        public int Nominal { get; set; }
        [Required]
        public decimal Value { get; set; }
    }
}