using AuroParser.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AuroParser
{
    public class CustomerStatementMessage
    {
        // Used by the Sender to unambiguously identify the message
        public string TransactionReference { get; private set; }
        // If the MT 940 is sent in response to an MT920 Request Message, this field must contain the field 20 Transaction Reference Number of the request message
        public string RelatedMessage { get; private set; }
        // This field identifies the account for which the statement is sent
        public string Account { get; private set; }
        // Sequential number of the statement, optionally followed by the sequence number of the message within that statement when more than one message is sent for one statement
        public string StatementNumber { get; private set; }
        // Indicating for the (intermediate) opening balance, whether it is a debit or credit balance, the date, the currency and the amount of the balance
        public TransactionBalance OpeningBalance { get; private set; }
        // Indicating for the (intermediate) closing balance, whether it is a debit or credit balance, the date, the currency and the amount of the balance
        public TransactionBalance ClosingBalance { get; private set; }
        // Indicates the funds which are available to the account owner (if credit balance) or the balance which is subject to interest charges (if debit balance)
        public TransactionBalance ClosingAvailableBalance { get; private set; }
        // Indicates the funds which are available to the account owner (if a credit or debit balance) for the specified forward value date
        public TransactionBalance ForwardAvailableBalance { get; private set; }

        public List<Transaction> Transactions { get; private set; }

        private CultureInfo _cultureInfo;

        public CustomerStatementMessage(string messageData, Mt940Parameters parameters)
        {
            _cultureInfo = parameters.Culture;

            var tagsStringArray = ConvertMt940DataToStringArray(messageData);
            var tagsTypedValues = ConvertMt940StringArrayToTypedValuesArray(tagsStringArray, parameters);
            var tagsTransactions = tagsTypedValues.Where(x => x.type == Mt940TagType.Tag61 || x.type == Mt940TagType.Tag86).ToArray();
            List<(string valueStatement, string valueDescription)> tagsTransactionPairs = null;
            if (tagsTransactions.Length % 2 == 0)
            {
                tagsTransactionPairs = new List<(string valueStatement, string valueDescription)>();
                for (int i = 0; i < tagsTransactions.Count(); i += 2)
                {
                    tagsTransactionPairs.Add((tagsTransactions[i].value, tagsTransactions[i + 1].value));
                }
            }
            else
            {
                throw new Exception("Cannot parse file with mt940 data. Count of tags :61: not equal to count of tags :86:");
            }

            foreach (var tag in parameters.TagDefinisions)
            {
                switch (tag.Type)
                {
                    case Mt940TagType.Tag20:
                        SetCustomerStatementMessageValues(tagsTypedValues, tag);
                        break;

                    case Mt940TagType.Tag21:
                        SetCustomerStatementMessageValues(tagsTypedValues, tag);
                        break;

                    case Mt940TagType.Tag25:
                        SetCustomerStatementMessageValues(tagsTypedValues, tag);
                        break;

                    case Mt940TagType.Tag28:
                    case Mt940TagType.Tag28C:
                        SetCustomerStatementMessageValues(tagsTypedValues, tag);
                        break;

                    case Mt940TagType.Tag60F:
                    case Mt940TagType.Tag60M:
                        if (tagsTypedValues.Where(x => x.type == tag.Type && !string.IsNullOrWhiteSpace(x.value)).Count() > 0)
                            SetCustomerStatementMessageValues(tagsTypedValues, tag);
                        break;

                    case Mt940TagType.Tag61:
                        SetTransactions(tagsTransactionPairs, tag, parameters.DescriptionParser);
                        break;

                    case Mt940TagType.Tag86:
                        SetTransactions(tagsTransactionPairs, tag, parameters.DescriptionParser);
                        break;

                    case Mt940TagType.Tag62F:
                    case Mt940TagType.Tag62M:
                        if(tagsTypedValues.Where(x => x.type == tag.Type && !string.IsNullOrWhiteSpace(x.value)).Count() > 0)
                            SetCustomerStatementMessageValues(tagsTypedValues, tag);
                        break;

                    case Mt940TagType.Tag64:
                        SetCustomerStatementMessageValues(tagsTypedValues, tag);
                        break;

                    case Mt940TagType.Tag65:
                        SetCustomerStatementMessageValues(tagsTypedValues, tag);
                        break;
                }
            }
        }

        private ICollection<string> ConvertMt940DataToStringArray(string data)
        {
            // Split on the new line seperator. In a MT940 messsage, every command is on a seperate line.
            // Assumption is made it is in the same format as the enviroments new line.
            var lines = data.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            // Create  an empty list of tags.
            var tags = new List<string>();

            string currentTag = null;
            foreach (var line in lines)
            {
                if (line.StartsWith(":", StringComparison.Ordinal))
                {
                    if (currentTag != null)
                    {
                        tags.Add(currentTag);
                        currentTag = line;

                        if (line.Equals(lines.Last()))
                            tags.Add(currentTag);
                    }
                    else
                    {
                        currentTag = line;
                    }
                }
                else
                {
                    if (currentTag != null)
                    {
                        currentTag += Environment.NewLine + line;
                    }
                }
            }
            return tags;
        }

        private ICollection<(Mt940TagType type, string value)> ConvertMt940StringArrayToTypedValuesArray(ICollection<string> dataArray, Mt940Parameters parameters)
        {
            var typedValues = new List<(Mt940TagType type, string value)?>();
            foreach (var tag in dataArray)
            {
                var tagName = (tag[0] == ':' && tag[3] == ':') ? tag.Substring(0, 4) : tag.Substring(0, 5);
                if (parameters.TagDefinisions.Where(x => EnumHelper.GetDescription(x.Type) == tagName) != null)
                {
                    var type = parameters.TagDefinisions.First(x => EnumHelper.GetDescription(x.Type) == tagName).Type;
                    var value = tag.Replace(tagName, "");
                    typedValues.Add((type, value));
                }
            }

            Mt940TagType? prevType = null;
            var tempTypedValues = new List<(Mt940TagType type, string value)?>(typedValues);
            foreach (var typedValue in typedValues.Select((value, i) => new { value, i }))
            {
                if (parameters.TagDefinisions.FirstOrDefault(x => x.Type == typedValue.value.Value.type && x.MultipleTag == true) != null)
                {
                    if (prevType != null)
                    {
                        if (typedValue.value.Value.type == prevType)
                        {
                            int prevIndex = typedValue.i - 1;
                            if (prevIndex > 0)
                            {
                                tempTypedValues[prevIndex] = null;
                            }
                        }
                    }
                }
                prevType = typedValue.value.Value.type;
            }

            var clearTypedValues = new List<(Mt940TagType type, string value)>();
            foreach (var typedValue in tempTypedValues)
            {
                if (typedValue != null)
                {
                    clearTypedValues.Add((typedValue.Value.type, typedValue.Value.value));
                }
            }
            return clearTypedValues;
        }

        private Match ParseTagValue(string value, string pattern)
        {
            var groupNamesMatch = Regex.Match(value, pattern);
            return groupNamesMatch ?? null;
        }

        private void SetCustomerStatementMessageValues(ICollection<(Mt940TagType type, string value)> typedTagsArray, TagDefinition tag)
        {
            var match = ParseTagValue(typedTagsArray.FirstOrDefault(x => x.type == tag.Type).value, tag.RegexPatternWithGroupNames);
            foreach (var type in tag.TagDataTypes)
            {
                switch (type)
                {
                    case Mt940DataType.TransactionReference:
                        TransactionReference = match.Groups["TransactionReference"].Value;
                        break;
                    case Mt940DataType.RelatedMessage:
                        RelatedMessage = match.Groups["RelatedMessage"].Value;
                        break;
                    case Mt940DataType.Account:
                        Account = match.Groups["Account"].Value;
                        break;
                    case Mt940DataType.StatementNumber:
                        StatementNumber = match.Groups["StatementNumber"].Value;
                        break;
                    case Mt940DataType.OpeningBalance:
                        OpeningBalance = new TransactionBalance(match.Groups["OpeningBalance"].Value, tag.RegexPatternWithGroupNames, type, _cultureInfo);
                        break;
                    case Mt940DataType.ClosingBalance:
                        ClosingBalance = new TransactionBalance(match.Groups["ClosingBalance"].Value, tag.RegexPatternWithGroupNames, type, _cultureInfo);
                        break;
                    case Mt940DataType.ClosingAvailableBalance:
                        ClosingAvailableBalance = new TransactionBalance(match.Groups["ClosingAvailableBalance"].Value, tag.RegexPatternWithGroupNames, type, _cultureInfo);
                        break;
                    case Mt940DataType.ForwardAvailableBalance:
                        ForwardAvailableBalance = new TransactionBalance(match.Groups["ForwardAvailableBalance"].Value, tag.RegexPatternWithGroupNames, type, _cultureInfo);
                        break;
                }
            }
        }

        private void SetTransactions(ICollection<(string valueStatement, string valueDescription)> transactionPairs, TagDefinition tag, DescriptionParser descParser)
        {
            if (tag.Type == Mt940TagType.Tag61)
            {
                if (Transactions == null)
                {
                    Transactions = new List<Transaction>();
                    foreach (var pair in transactionPairs)
                    {
                        var transaction = new Transaction(OpeningBalance.Currency, _cultureInfo);
                        transaction.SetStatement(pair.valueStatement, tag);
                        Transactions.Add(transaction);
                    }
                }
                else
                {
                    foreach (var pair in transactionPairs.Select((value, i) => new { value, i }))
                    {
                        var transaction = Transactions[pair.i];
                        Transactions[pair.i].SetStatement(pair.value.valueStatement, tag);
                    }
                }
            }

            if (tag.Type == Mt940TagType.Tag86)
            {
                if (Transactions == null)
                {
                    Transactions = new List<Transaction>();
                    foreach (var pair in transactionPairs)
                    {
                        var transaction = new Transaction(OpeningBalance.Currency, _cultureInfo);
                        transaction.SetDescription(pair.valueDescription, tag, descParser);
                        Transactions.Add(transaction);
                    }
                }
                else
                {
                    foreach (var pair in transactionPairs.Select((value, i) => new { value, i }))
                    {
                        var transaction = Transactions[pair.i];
                        transaction.SetDescription(pair.value.valueDescription, tag, descParser);
                    }
                }
            }
        }
    }
}
