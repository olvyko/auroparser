using AuroParser.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AuroParser
{
    public class TransactionDescription
    {
        public string Value { get; private set; }

        // Account number
        public string AccountString { get; private set; }
        public string NameString { get; private set; }
        public string AdressString { get; private set; }
        public string InformationString { get; private set; }
        public string NameAdressString { get; private set; }
        public string NameAdressInformationString { get; private set; }

        public string ValidatedDescription { get; set; }

        public TransactionDescription(string tagValue, TagDefinition property, DescriptionParser descParser)
        {
            var groupNamesMatch = Regex.Match(tagValue, property.RegexPatternWithGroupNames, RegexOptions.Singleline);
            var descriptionData = groupNamesMatch.Groups["TransactionDescription"].Value;
            Value = descriptionData;
            var parsedDescription = descParser.ParseDescription(descriptionData);
            if (parsedDescription != null)
            {
                foreach (var item in parsedDescription)
                {
                    SetTransactionDescriptionValue(item.type, item.value);
                }
            }
        }

        private void SetTransactionDescriptionValue(Mt940DescriptionDataType type, string value)
        {
            switch (type)
            {
                case Mt940DescriptionDataType.DescriptionAccountString:
                    AccountString = value;
                    break;
                case Mt940DescriptionDataType.DescriptionNameString:
                    NameString = value;
                    break;
                case Mt940DescriptionDataType.DescriptionAdressString:
                    AdressString = value;
                    break;
                case Mt940DescriptionDataType.DescriptionInformationString:
                    InformationString = value;
                    break;
                case Mt940DescriptionDataType.DescriptionNameAdressString:
                    NameAdressString = value;
                    break;
                case Mt940DescriptionDataType.DescriptionNameAdressInformationString:
                    NameAdressInformationString = value;
                    break;
            }
        }
    }
}
