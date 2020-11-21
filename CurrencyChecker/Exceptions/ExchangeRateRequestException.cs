using System;

namespace CurrencyChecker.Exceptions
{
    public class ExchangeRateRequestException : Exception
    {
        public ExchangeRateRequestException(string message) : base(message)
        { }
    }
}