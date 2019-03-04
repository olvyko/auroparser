using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroParser
{
    public static class DateParser
    {
        public static DateTime ParseDate(string year, string month, string day, CultureInfo cultureInfo)
        {
            if (String.IsNullOrWhiteSpace(year))
            {
                throw new ArgumentException("year can not be empty", nameof(year));
            }

            if (String.IsNullOrWhiteSpace(month))
            {
                throw new ArgumentException("month can not be empty", nameof(month));
            }

            if (String.IsNullOrWhiteSpace(day))
            {
                throw new ArgumentException("day can not be empty", nameof(day));
            }

            if (cultureInfo == null)
            {
                throw new ArgumentNullException(nameof(cultureInfo));
            }

            var parsedFourDigitYear = cultureInfo.Calendar.ToFourDigitYear(ValueConverter.ParseInteger(year, cultureInfo));
            var parsedMonth = ValueConverter.ParseInteger(month, cultureInfo);
            var parsedDay = ValueConverter.ParseInteger(day, cultureInfo);

            return new DateTime(parsedFourDigitYear, parsedMonth, parsedDay);
        }
    }
}
