using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace maumx.Bitso.Entities.Response
{
    internal sealed class GeneralResponse<T>
        where T:class
    {

        public GeneralResponse()
        {
            PayLoad = null;
            Error = null;
        }
        public Boolean Success { get; set; }
        public T PayLoad { get; set;}
        public ErrorResponse Error { get; set; }
    }


    public class ErrorResponse
    {
        public string Message { get; set; }
        public int   Code { get; set; }

    }
}
