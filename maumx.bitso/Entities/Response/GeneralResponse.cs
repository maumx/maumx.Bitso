using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace maumx.Bitso.Entities.Response
{
    internal sealed class GeneralResponse<T>
        where T:BitsoEntity, new()
    {

        public GeneralResponse()
        {
            PayLoad = new T();
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
