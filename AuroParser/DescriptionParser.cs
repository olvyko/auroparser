using AuroParser.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AuroParser
{
    public class DescriptionParser
    {
        public DescriptionSeparatorType DescriptionType { get; private set; }
        public int MaxLineLength { get; private set; }
        public Dictionary<Mt940DescriptionDataType, List<string>> TypedDescriptionSeparators { get; private set; }
        public Dictionary<Mt940DescriptionDataType, List<int>> TypedDescriptionOrder { get; private set; }

        public DescriptionParser(Dictionary<Mt940DescriptionDataType, List<string>> descriptionSeparators, int maxLineLength)
        {
            MaxLineLength = maxLineLength;
            TypedDescriptionSeparators = descriptionSeparators;
            DescriptionType = DescriptionSeparatorType.Unique;
        }

        public DescriptionParser(Dictionary<Mt940DescriptionDataType, List<int>> descriptionOrder, int maxLineLength)
        {
            MaxLineLength = maxLineLength;
            TypedDescriptionOrder = descriptionOrder;
            DescriptionType = DescriptionSeparatorType.NewLine;
        }

        public ICollection<(Mt940DescriptionDataType type, string value)> ParseDescription(string data)
        {
            switch (DescriptionType)
            {
                case DescriptionSeparatorType.Unique:
                    return ParseDescriptionUnique(data);
                case DescriptionSeparatorType.NewLine:
                    return ParseDescriptionNewLine(data);
                default:
                    return null;
            }
        }

        private ICollection<(Mt940DescriptionDataType type, string value)> ParseDescriptionUnique(string data)
        {
            data += Environment.NewLine;
            var parsedDesc = new List<(Mt940DescriptionDataType type, string value)>();
            foreach (var typedSeparator in TypedDescriptionSeparators)
            {
                string value = "";
                foreach (string separator in typedSeparator.Value)
                {
                    var pattern = separator + @"(?<value>.+)(\r\n)";
                    var match = Regex.Match(data, pattern, RegexOptions.Multiline);
                    var tempValue = match.Groups["value"].Value;
                    if (!string.IsNullOrWhiteSpace(tempValue))
                    {
                        value += match.Groups["value"].Value.TrimStart().TrimEnd() + Environment.NewLine;
                    }
                }
                value = value.TrimEnd(Environment.NewLine.ToCharArray());
                parsedDesc.Add((typedSeparator.Key, value));
            }
            return parsedDesc.Count != 0 ? parsedDesc : null;
        }

        private ICollection<(Mt940DescriptionDataType type, string value)> ParseDescriptionNewLine(string data)
        {
            var parsedDesc = new List<(Mt940DescriptionDataType type, string value)>();
            string[] splitedData = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var typedOrder in TypedDescriptionOrder)
            {
                string value = "";
                // If list have one negative -> need to fill from +{this} to end
                // If list have two negative -> need to fill from +{this} to -{this}
                var negativeNumbers = typedOrder.Value.Where(x => x < 0).ToArray();
                if (negativeNumbers.Length == 0)
                {
                    foreach (int index in typedOrder.Value)
                    {
                        if (inBounds(index, splitedData))
                        {
                            value += splitedData[index] + Environment.NewLine;
                        }
                    }
                    value = value.TrimEnd(Environment.NewLine.ToCharArray());
                    parsedDesc.Add((typedOrder.Key, value));
                }
                else if (negativeNumbers.Length == 1)
                {
                    int index = toPositive(negativeNumbers[0]);
                    if (inBounds(index, splitedData))
                    {
                        for (int i = index; i < splitedData.Length; i++)
                        {
                            value += splitedData[i] + Environment.NewLine;
                        }
                    }
                    value = value.TrimEnd(Environment.NewLine.ToCharArray());
                    parsedDesc.Add((typedOrder.Key, value));
                }
                else if (negativeNumbers.Length == 2)
                {
                    int index1 = toPositive(negativeNumbers[0]);
                    int index2 = toPositive(negativeNumbers[1]);
                    if (inBounds(index1, splitedData) && inBounds(index2, splitedData))
                    {
                        for (int i = index1; i < index2; i++)
                        {
                            value += splitedData[i] + Environment.NewLine;
                        }
                    }
                }
            }
            return parsedDesc.Count != 0 ? parsedDesc : null;
        }

        private bool inBounds<T>(int index, T[] array)
        {
            return (index >= 0) && (index < array.Length);
        }
        
        private int toPositive(int number)
        {
            return Math.Abs(number);
        }

        private int toNegative(int number)
        {
            return Math.Abs(number) * -1;
        }
    }
}
