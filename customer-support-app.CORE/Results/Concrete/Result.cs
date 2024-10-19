using customer_support_app.CORE.Results.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.Results.Concrete
{
    public class Result : IResult
    {
        public bool Success { get; }
        public string Message { get; }
        public int Code { get; }

        public Result(bool success, string message, int code) : this(success)
        {
            Message = message;
            Code = code;
        }

        public Result(bool success)
        {
            Success = success;
        }

    }
}
