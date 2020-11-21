using System;

namespace CurrencyChecker.Exceptions
{
    public class ExchangeRatesSaveException : Exception
    {
        public ExchangeRatesSaveException(string message) : base(message)
        { }
    }
}