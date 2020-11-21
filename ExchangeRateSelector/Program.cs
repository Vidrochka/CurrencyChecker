using NLog;
using SharedModels;
using System;
using System.Globalization;
using System.Linq;

namespace ExchangeRateSelector
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Logger logger = LogManager.GetCurrentClassLogger();

            logger.Info("Program start");
            logger.Info("Введите дату в формате yyyy.MM.dd");


            string stringSearchDate = string.Empty;
            DateTime searchDate;
            while (true)
            {
                Console.Write(">");
                stringSearchDate = Console.ReadLine();

                if (DateTime.TryParseExact(stringSearchDate, "yyyy.MM.dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out searchDate))
                {
                    logger.Info($"Поиск за дату [{searchDate:yyyy.MM.dd}]");
                    break;
                }

                logger.Warn($"Неверная дата [{stringSearchDate}] необходимо [yyyy.MM.dd]");
            }

            logger.Info("Введите сокращенное название валюты");

            Console.Write(">");
            string searchValuteName = Console.ReadLine();
            logger.Info($"Поиск по назвнию [{searchValuteName}]");

            using ExchangeRatesDdContext db = new ExchangeRatesDdContext(logger, "Data Source=(localdb)\\MSSQLLocalDB;Database=Currency;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");


            IQueryable<ExchangeRates> valutes = db.ExchangeRates.Where(valute =>
                valute.CharCode == searchValuteName && valute.ExchangeRateDate == searchDate);

            if (valutes.Count() == 0)
            {
                logger.Info("Информация не найдена");
            }
            else
            {
                ExchangeRates valute = valutes.Single();
                logger.Info($"{Environment.NewLine}Номинал: [{valute.Nominal}]{Environment.NewLine}" +
                            $"Курс: [{valute.Value}]");
            }

            Console.WriteLine("Нажмите [ENTER для выхода]");
            Console.ReadLine();
        }
    }
}
