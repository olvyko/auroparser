using System;
using System.Globalization;

namespace AuroParser
{
    public static class ValueConverter
    {
        public static int ParseInteger(string value, IFormatProvider cultureInfo)
        {
            if (TryParseInteger(value, cultureInfo, out int result))
                return result;

            throw new InvalidCastException();
        }

        public static bool TryParseInteger(string integer, IFormatProvider cultureInfo, out int result)
        {
            return int.TryParse(integer, NumberStyles.Any, cultureInfo, out result);
        }

        public static decimal ParseDecimal(string value, IFormatProvider cultureInfo)
        {
            if (TryParseDecimal(value, cultureInfo, out decimal result))
                return result;

            throw new InvalidCastException();
        }

        public static bool TryParseDecimal(string dec, IFormatProvider cultureInfo, out decimal result)
        {
            return decimal.TryParse(dec, NumberStyles.Any, cultureInfo, out result);
        }
    }
}
