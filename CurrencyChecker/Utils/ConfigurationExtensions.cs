using CurrencyChecker.Exceptions;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;

namespace CurrencyChecker.Utils
{
    public static class ConfigurationExtensions
    {
        public const string LoadTimeMask = "g";

        #region SettingsName
        public const string RequestUri = "RequestURI";
        public const string WebRequestTimeout = "WebRequestTimeout";
        public const string RetryRequestCount = "RetryRequestCount";
        public const string RetryRequestWaitTime = "RetryRequestWaitTime";

        public const string DdConnectionString = "DBConnectionString";
        public const string RetryStoreCurrencyCount = "RetryStoreCurrencyCount";
        public const string RetryStoreCurrencyWaitTime = "RetryStoreCurrencyWaitTime";

        public const string CurrencyLoadTime = "CurrencyLoadTime";
        public const string NeedImmediateLoad = "NeedImmediateLoad";
        #endregion

        #region WebCfg
        public static string GetRequestUri(this IConfiguration configuration)
            => configuration.GetValue<string>(RequestUri);

        public static int GetWebRequestTimeout(this IConfiguration configuration)
            => configuration.GetValue<int>(WebRequestTimeout);

        public static int GetRetryRequestCount(this IConfiguration configuration)
            => configuration.GetValue<int>(RetryRequestCount);

        public static TimeSpan GetRetryRequestWaitTime(this IConfiguration configuration)
        {
            if (TimeSpan.TryParseExact(configuration.GetValue<string>(RetryRequestWaitTime),
                LoadTimeMask, CultureInfo.CurrentCulture, out TimeSpan time))
            {
                return time;
            }

            throw new ConfigurationLoadException($"{RetryRequestWaitTime} is not valid parameter [{LoadTimeMask}] [{configuration.GetValue<string>(RetryRequestWaitTime)}]");
        }
        #endregion

        #region DbCfg
        public static string GetDdConnectionString(this IConfiguration configuration)
            => configuration.GetValue<string>(DdConnectionString);

        public static int GetRetryStoreCurrencyCount(this IConfiguration configuration)
            => configuration.GetValue<int>(RetryStoreCurrencyCount);

        public static TimeSpan GetRetryStoreCurrencyWaitTime(this IConfiguration configuration)
        {
            if (TimeSpan.TryParseExact(configuration.GetValue<string>(RetryStoreCurrencyWaitTime),
                LoadTimeMask, CultureInfo.CurrentCulture, out TimeSpan time))
            {
                return time;
            }

            throw new ConfigurationLoadException($"{RetryStoreCurrencyWaitTime} is not valid parameter [{LoadTimeMask}] [{configuration.GetValue<string>(RetryStoreCurrencyWaitTime)}]");
        }
        #endregion

        #region LoadCfg
        public static long GetCurrencyLoadTime(this IConfiguration configuration)
        {
            if (TimeSpan.TryParseExact(configuration.GetValue<string>(CurrencyLoadTime),
                LoadTimeMask, CultureInfo.CurrentCulture, out TimeSpan time))
            {
                return Convert.ToInt64(time.TotalSeconds);
            }

            throw new ConfigurationLoadException($"{CurrencyLoadTime} is not valid parameter [{LoadTimeMask}] [{configuration.GetValue<string>(CurrencyLoadTime)}]");
        }

        public static bool GetNeedImmediateLoad(this IConfiguration configuration)
            => configuration.GetValue<bool>(NeedImmediateLoad);
        #endregion
    }
}