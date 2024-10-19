using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.Results.Abstract
{
    public interface IDataResult<out T>:IResult
    {
        T Data { get; }  
    }
}
