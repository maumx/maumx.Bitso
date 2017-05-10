using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace maumx.Bitso.Entities.Core
{
  public  class Balance: BitsoEntity
    {

        public Balance()
        {

        }
        public string Currency { get; set; }
        public decimal Total { get; set; }
        public decimal Locked { get; set; }
        public decimal Available { get; set; }
    }


}
