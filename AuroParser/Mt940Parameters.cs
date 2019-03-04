using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AuroParser.DataTypes.Enums;

namespace AuroParser
{
    public class Mt940Parameters
    {
        public ICollection<TagDefinition> TagDefinisions { get; private set; }
        public Mt940KeyDataType UniqueKeyDataType { get; private set; }
        public DescriptionParser DescriptionParser { get; private set; }
        public CultureInfo Culture { get; private set; }
        public bool IsHeaderTrailerExists => !string.IsNullOrWhiteSpace(Header) && !string.IsNullOrWhiteSpace(Trailer);
        public string Header { get; private set; }
        public string Trailer { get; private set; }

        public Mt940Parameters(ICollection<TagDefinition> tagDefinisions, CultureInfo culture, DescriptionParser descriptionParser, string header, string trailer)
            : this(tagDefinisions, culture, descriptionParser)
        {
            Header = header;
            Trailer = trailer;
        }

        public Mt940Parameters(ICollection<TagDefinition> tagDefinisions, CultureInfo culture, DescriptionParser descriptionParser)
        {
            if (culture == null)
                throw new NullReferenceException("culture cannot be null");
            if (tagDefinisions == null)
                throw new NullReferenceException("tagDefinisions cannot be null");
            if (tagDefinisions.Count == 0)
                throw new NullReferenceException("tagDefinisions cannot be empty");
            if (descriptionParser == null)
                throw new NullReferenceException("descriptionParser cannot be null");

            DescriptionParser = descriptionParser;
            TagDefinisions = tagDefinisions;
            Culture = culture;
        }

        public void SetUniqueKeyDataType(Mt940KeyDataType keyDataType)
        {
            UniqueKeyDataType = keyDataType;
        }
    }
}
