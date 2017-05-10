using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using maumx.Bitso.Entities.Core;

namespace maumx.Bitso.Entities
{
    public class UserBalance :BitsoEntity
    {
        public UserBalance()
        {

        }
        public Balance[] Balances { get; set;}
    }
}
