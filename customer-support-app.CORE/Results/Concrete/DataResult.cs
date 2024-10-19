using customer_support_app.CORE.Results.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.Results.Concrete
{
    public class DataResult<T> : Result, IDataResult<T>
    {
        public T Data { get; }

        public DataResult(T data, bool success, string message, int code) : base(success, message, code)
        {
            Data = data;
        }

        public DataResult(T data, bool success, int code) : base(success)
        {
            Data = data;
        }
    }
}
