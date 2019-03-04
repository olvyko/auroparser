using AuroParser.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AuroParser
{
    public class TransactionBalance
    {
        public DebitCreditType DebitCredit { get; private set; }
        public DateTime? EntryDate { get; private set; }
        public Currency Currency { get; private set; }
        public Money Balance { get; private set; }

        public TransactionBalance(string data, string pattern, Mt940DataType type, CultureInfo cultureInfo)
        {
            if (String.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentException("data can not be empty", data);
            }

            if (String.IsNullOrWhiteSpace(pattern))
            {
                throw new ArgumentException("pattern can not be empty", pattern);
            }

            if (cultureInfo == null)
            {
                throw new ArgumentNullException(nameof(cultureInfo));
            }

            var match = Regex.Match(data, pattern);
            this.DebitCredit = DebitCreditFactory.Create(match.Groups[type.ToString() + "DebitCreditMark"].Value);
            this.EntryDate = ExtractEntryDate(match.Groups[type.ToString() + "EntryDate"].Value, cultureInfo);
            this.Currency = new Currency(match.Groups[type.ToString() + "Currency"].Value);
            this.Balance = new Money(match.Groups[type.ToString() + "Amount"].Value, Currency, cultureInfo);
        }

        private static DateTime? ExtractEntryDate(string data, CultureInfo cultureInfo)
        {
            if (data == null)
            {
                return null;
            }

            if (cultureInfo == null)
            {
                throw new ArgumentNullException(nameof(cultureInfo));
            }

            var match = Regex.Match(data, @"(?<year>\d{2})(?<month>\d{2})(?<day>\d{2})");
            return DateParser.ParseDate(match.Groups["year"].Value, match.Groups["month"].Value, match.Groups["day"].Value, cultureInfo);
        }
    }
}
