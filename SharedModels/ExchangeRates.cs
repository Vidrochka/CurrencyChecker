using System;
using System.ComponentModel.DataAnnotations;

namespace SharedModels
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
