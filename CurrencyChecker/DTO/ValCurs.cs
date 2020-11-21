using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace CurrencyChecker.DTO
{
    [XmlRoot]
    [Serializable]
    public class ValCurs
    {
        [XmlIgnore]
        public DateTime Date { get; set; }

        [XmlAttribute(nameof(Date))]
        public string StringDate
        {
            get => $"{Date:yyyy.MM.dd}";
            set => Date = DateTime.ParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
        }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("Valute")]
        public List<Valute> Valute { get; set; }
    }
}