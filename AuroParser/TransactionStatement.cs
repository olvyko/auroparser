using AuroParser.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AuroParser
{
    public class TransactionStatement
    {
        public string Value { get; private set; }

        // Value date of the transaction
        public DateTime ValueDate { get; private set; }
        // Entry date of the transaction
        public DateTime? EntryDate { get; private set; }
        // Debit/Credit Mark
        public DebitCreditType? DebitCreditMark { get; private set; }
        // 3rd character of the currency code, if needed
        public string FundsCode { get; private set; }
        // Amount of the transaction
        public Money Amount { get; private set; }
        // Transaction Type Identification Code
        public string TransactionType { get; private set; }
        // Customer reference
        public string CustomerReference { get; private set; }
        // Bank reference
        public string BankReference { get; private set; }
        // Subfield supplementary details
        public string SupplementaryDetails { get; private set; }

        private Currency _currency;
        private CultureInfo _cultureInfo;

        public TransactionStatement(string tagValue, TagDefinition property, Currency currency, CultureInfo cultureInfo)
        {
            _currency = currency;
            _cultureInfo = cultureInfo;

            var groupNamesMatch = Regex.Match(tagValue, property.RegexPatternWithGroupNames);
            foreach (var type in property.TagDataTypes)
            {
                SetTransactionStatementValue(type, groupNamesMatch);
            }
        }

        private void SetTransactionStatementValue(Mt940DataType type, Match match)
        {
            switch (type)
            {
                case Mt940DataType.TransactionStatementLine:
                    Value = match.Groups["TransactionStatementLine"].Value;
                    break;
                case Mt940DataType.ValueDate:
                    ValueDate = ExtractValueDate(match.Groups["ValueDate"].Value, _cultureInfo);
                    break;
                case Mt940DataType.EntryDate:
                    EntryDate = ExtractEntryDate(match.Groups["EntryDate"].Value, ValueDate.Year.ToString(), _cultureInfo);
                    break;
                case Mt940DataType.DebitCreditMark:
                    DebitCreditMark = DebitCreditFactory.Create(match.Groups["DebitCreditMark"].Value);
                    break;
                case Mt940DataType.FundsCode:
                    FundsCode = match.Groups["FundsCode"].Value;
                    break;
                case Mt940DataType.Amount:
                    Amount = new Money(match.Groups["Amount"].Value, _currency, _cultureInfo);
                    break;
                case Mt940DataType.TransactionType:
                    TransactionType = match.Groups["TransactionType"].Value;
                    break;
                case Mt940DataType.CustomerReference:
                    CustomerReference = match.Groups["CustomerReference"].Value;
                    break;
                case Mt940DataType.BankReference:
                    BankReference = match.Groups["BankReference"].Value;
                    break;
                case Mt940DataType.SupplementaryDetails:
                    SupplementaryDetails = match.Groups["SupplementaryDetails"].Value;
                    break;
            }
        }

        private static DateTime ExtractValueDate(string data, CultureInfo cultureInfo)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (cultureInfo == null)
            {
                throw new ArgumentNullException(nameof(cultureInfo));
            }

            var match = Regex.Match(data, @"(?<year>\d{2})(?<month>\d{2})(?<day>\d{2})");
            return DateParser.ParseDate(match.Groups["year"].Value, match.Groups["month"].Value, match.Groups["day"].Value, cultureInfo);
        }

        private static DateTime? ExtractEntryDate(string data, string year, CultureInfo cultureInfo)
        {
            if (data == null)
            {
                return null;
            }

            if (cultureInfo == null)
            {
                throw new ArgumentNullException(nameof(cultureInfo));
            }

            var match = Regex.Match(data, @"(?<month>\d{2})(?<day>\d{2})");
            var entryDate = DateParser.ParseDate(year, match.Groups["month"].Value, match.Groups["day"].Value, cultureInfo);
            return entryDate;
        }
    }
}
