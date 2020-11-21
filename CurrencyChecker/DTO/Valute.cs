using System;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace CurrencyChecker.DTO
{
    [Serializable]
    public class Valute
    {
        [XmlAttribute(nameof(ID))]
        public string ID { get; set; }

        [XmlElement(nameof(NumCode))]
        public string NumCode { get; set; }
        [XmlElement(nameof(CharCode))]
        public string CharCode { get; set; }
        [XmlElement(nameof(Nominal))]
        public int Nominal { get; set; }
        [XmlElement(nameof(Name))]
        public string Name { get; set; }

        [XmlIgnore]
        public decimal Value { get; set; }
        [XmlElement(nameof(Value))]
        public string StringValue
        {
            get => Convert.ToString(Value, CultureInfo.InvariantCulture);
            set => Value = decimal.Parse(value.Replace(',', '.'), CultureInfo.InvariantCulture);
        }
    }
}