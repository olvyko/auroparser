using AuroParser.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroParser
{
    public static class DebitCreditFactory
    {
        public static DebitCreditType Create(string type)
        {
            switch (type)
            {
                case "D":
                    return DebitCreditType.Debit;
                case "C":
                    return DebitCreditType.Credit;
                case "RC":
                    return DebitCreditType.RC;
                case "RD":
                    return DebitCreditType.RD;
            }

            throw new ArgumentException(type, nameof(type));
        }
    }
}
