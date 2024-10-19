using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.Results.Concrete
{
    public class ErrorDataResult<T> : DataResult<T>
    {
        public ErrorDataResult(T data, string message, int code) : base(data, false, message, code)
        {

        }
        public ErrorDataResult(T data, int code) : base(data, false, default, code)
        {

        }
        public ErrorDataResult(string message, int code) : base(default, false, message, code)
        {

        }
        public ErrorDataResult() : base(default, false, StatusCodes.Status500InternalServerError)
        {

        }
        public ErrorDataResult(T data) : base(data, false, default, StatusCodes.Status500InternalServerError)
        {

        }
    }
}
