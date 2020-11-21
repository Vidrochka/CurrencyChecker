using System;

namespace CurrencyChecker.Exceptions
{
    public class ConfigurationLoadException : Exception
    {
        public ConfigurationLoadException(string message) : base(message)
        { }
    }
}