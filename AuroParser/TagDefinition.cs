using AuroParser.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroParser
{
    public class TagDefinition
    {
        public Mt940TagType Type { get; private set; }
        public ICollection<Mt940DataType> TagDataTypes { get; private set; }
        public bool MultipleTag { get; private set; }
        public string RegexPatternWithGroupNames { get; private set; }

        public TagDefinition(Mt940TagType tagType, ICollection<Mt940DataType> tagDataTypes, string regexPatternWithGroupNames, bool multipleTag = false)
        {
            if(tagDataTypes == null)
                throw new NullReferenceException("tagDataTypes cannot be null");
            if (tagDataTypes.Count == 0)
                throw new NullReferenceException("tagDataTypes cannot be empty");
            if (string.IsNullOrWhiteSpace(regexPatternWithGroupNames))
                throw new NullReferenceException("regexPatternWithGroupNames cannot be null or empty");

            Type = tagType;
            TagDataTypes = tagDataTypes;
            MultipleTag = multipleTag;
            RegexPatternWithGroupNames = regexPatternWithGroupNames;
        }
    }
}
