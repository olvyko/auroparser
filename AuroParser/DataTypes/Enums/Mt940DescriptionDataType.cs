using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroParser.DataTypes.Enums
{
    public enum Mt940DescriptionDataType
    {
        /* TransactionDescription parts */
        // Full account number of the transaction description
        DescriptionAccountString,
        DescriptionNameString,
        DescriptionAdressString,
        DescriptionInformationString,
        DescriptionNameAdressString,
        DescriptionNameAdressInformationString
    }
}
