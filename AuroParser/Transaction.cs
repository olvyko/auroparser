using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroParser
{
    public class Transaction
    {
        public TransactionStatement Statement { get; private set; }
        public TransactionDescription Description { get; private set; }

        private Currency _currency;
        private CultureInfo _cultureInfo;

        public Transaction(Currency currency, CultureInfo cultureInfo)
        {
            _currency = currency ?? throw new ArgumentNullException(nameof(currency));
            _cultureInfo = cultureInfo ?? throw new ArgumentNullException(nameof(cultureInfo));
        }

        public void SetStatement(string tagValue, TagDefinition property)
        {
            Statement = new TransactionStatement(tagValue, property, _currency, _cultureInfo);
        }

        public void SetDescription(string tagValue, TagDefinition property, DescriptionParser descParser)
        {
            Description = new TransactionDescription(tagValue, property, descParser);
        }
    }
}
