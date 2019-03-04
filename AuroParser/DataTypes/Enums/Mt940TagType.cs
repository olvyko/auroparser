using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroParser.DataTypes.Enums
{
    public enum Mt940TagType
    {
        [Description(":20:")]
        Tag20,
        [Description(":21:")]
        Tag21,
        [Description(":25:")]
        Tag25,
        [Description(":28:")]
        Tag28,
        [Description(":28C:")]
        Tag28C,
        [Description(":60F:")]
        Tag60F,
        [Description(":60M:")]
        Tag60M,
        [Description(":61:")]
        Tag61,
        [Description(":86:")]
        Tag86,
        [Description(":62F:")]
        Tag62F,
        [Description(":62M:")]
        Tag62M,
        [Description(":64:")]
        Tag64,
        [Description(":65:")]
        Tag65
    }
}
