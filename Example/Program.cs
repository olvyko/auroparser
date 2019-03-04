using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using AuroParser;
using AuroParser.DataTypes.Enums;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var data = @":20:MT940
:25:/PL80224030990000400208955593
:28C:180521
:60F:D180522PLN666666,66
:61:1805220522C241,08S15000000029217
:86:150
:86:150~00B150PRZELEW ZEWNĘTRZNY PRZY
~20test info
~21
~22
~23
~24
~25
~26
~27
~28
~2922222222222222222222222222
~3022222222
~312222222222222222
~32LOL LOL SP. Z O.O.        
~33        UL LOL 104
~34150
~38PL22222222222222222222222222
~6222-222 WARSZAWA
~63
:61:1805220522C6612,48S15000000029218
:86:150
:86:150~00B150PRZELEW ZEWNĘTRZNY PRZY
~20test info 2
~21
~22
~23
~24
~25
~26
~27
~28
~2922222222222222222222222222
~3022222222
~312222222222222222
~32OLOLLOL2            
~33        SP. Z O.O.
~34150
~38PL22222222222222222222222222
~62UL. LOL 3                
~63        22-222 WARSZAWA
:62F:D180522PLN222222,22";

            var tagDefinition = new List<TagDefinition>
            {
                new TagDefinition
                (
                    Mt940TagType.Tag20,
                    new List<Mt940DataType> { Mt940DataType.TransactionReference },
                    "(?<TransactionReference>.+)"
                ),
                new TagDefinition
                (
                    Mt940TagType.Tag25,
                    new List<Mt940DataType> { Mt940DataType.Account },
                    "/(?<Account>.+)"
                ),
                new TagDefinition
                (
                    Mt940TagType.Tag28C,
                    new List<Mt940DataType> { Mt940DataType.StatementNumber },
                    "(?<StatementNumber>.+)"
                ),
                new TagDefinition
                (
                    Mt940TagType.Tag60F,
                    new List<Mt940DataType> { Mt940DataType.OpeningBalance, Mt940DataType.OpeningBalanceAmount, Mt940DataType.OpeningBalanceEntryDate, Mt940DataType.OpeningBalanceCurrency, Mt940DataType.OpeningBalanceDebitCreditMark },
                    @"(?<OpeningBalance>(?<OpeningBalanceDebitCreditMark>C|D)(?<OpeningBalanceEntryDate>[0-9]{6})(?<OpeningBalanceCurrency>[A-Z]{3})(?<OpeningBalanceAmount>\d*,\d{0,2}))"
                ),
                new TagDefinition
                (
                    Mt940TagType.Tag61,
                    new List<Mt940DataType> { Mt940DataType.TransactionStatementLine, Mt940DataType.ValueDate, Mt940DataType.EntryDate, Mt940DataType.DebitCreditMark, Mt940DataType.Amount, Mt940DataType.TransactionType, Mt940DataType.CustomerReference },
                    @"(?<TransactionStatementLine>(?<ValueDate>\d{6})(?<EntryDate>\d{4})(?<DebitCreditMark>C|D|RC|RD)(?<Amount>\d*,\d{0,2})(?<TransactionType>[\w\s]{4})(?<CustomerReference>[\s\w]{0,11}))"
                ),
                new TagDefinition
                (
                    Mt940TagType.Tag86,
                    new List<Mt940DataType> { Mt940DataType.TransactionDescription },
                    @"\d{3}(?<TransactionDescription>.+)",
                    true
                ),
                new TagDefinition
                (
                    Mt940TagType.Tag62F,
                    new List<Mt940DataType> { Mt940DataType.ClosingBalance, Mt940DataType.ClosingBalanceAmount, Mt940DataType.ClosingBalanceEntryDate, Mt940DataType.ClosingBalanceCurrency, Mt940DataType.ClosingBalanceDebitCreditMark},
                    @"(?<ClosingBalance>(?<ClosingBalanceDebitCreditMark>C|D)(?<ClosingBalanceEntryDate>[0-9]{6})(?<ClosingBalanceCurrency>[A-Z]{3})(?<ClosingBalanceAmount>\d*,\d{0,2}))"
                )
            };
            var descParser = new DescriptionParser(new Dictionary<Mt940DescriptionDataType, List<string>>
            {
                { Mt940DescriptionDataType.DescriptionAccountString, new List<string> { "~38" } },
                { Mt940DescriptionDataType.DescriptionNameString, new List<string> { "~32", "~33" } },
                { Mt940DescriptionDataType.DescriptionAdressString, new List<string> { "~62", "~63" } },
                { Mt940DescriptionDataType.DescriptionInformationString, new List<string> { "~20", "~21", "~22", "~23", "~24", "~25", "~26", "~27", "~28" } }
            }, 54);
            var parameters = new Mt940Parameters(tagDefinition, new CultureInfo("pl"), descParser);
            var messages = AuroMt940Parser.ParseData(data, parameters).ToList();

            Console.WriteLine($"Account: {messages.FirstOrDefault().Account}");
            Console.WriteLine($"Statement Number: {messages.FirstOrDefault().StatementNumber}");
            Console.WriteLine($"Count of transactions: {messages.FirstOrDefault().Transactions.Count}");
            Console.ReadKey();
        }
    }
}
