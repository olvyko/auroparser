using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroParser
{
    public class Currency
    {
        public string Code { get; private set; }

        public Currency(string currencyCode)
        {
            if (string.IsNullOrEmpty(currencyCode))
            {
                throw new ArgumentException("Currency code can not be empty.", currencyCode);
            }

            this.Code = currencyCode;
        }
    }
}
