using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroParser
{
    public class Money
    {
        public Currency Currency { get; private set; }
        public decimal Value { get; private set; }

        public Money(string value, Currency currency, IFormatProvider cultureInfo)
        {
            if (cultureInfo == null)
            {
                throw new ArgumentNullException(nameof(cultureInfo));
            }

            if (currency == null)
            {
                throw new ArgumentNullException(nameof(currency));
            }

            if (String.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("value can not be empty", value);
            }

            var formattedValue = string.Format(cultureInfo, "{0:C}", value);
            this.Value = ValueConverter.ParseDecimal(formattedValue, cultureInfo);
            this.Currency = currency;
        }

        public override string ToString()
        {
            return $"{Currency.Code} {Value}";
        }
    }
}
