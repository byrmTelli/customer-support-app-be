using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.Results.Concrete
{
    public class ErrorResult : Result
    {
        public List<string>? Errors { get; set; }
        public ErrorResult() : base(false)
        {
            Errors = null;
        }

        public ErrorResult(string message, int code) : base(false, message, code)
        {
            Errors = new List<string>();
        }

        public ErrorResult(string message, int code, List<string>? errors) : base(false, message, code)
        {
            Errors = errors ?? null;
        }
    }
}
