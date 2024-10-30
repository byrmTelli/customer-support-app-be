using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.ViewModels.Admin.Dashboard
{
    public class MonthlySupportRequest
    {
        public string Month { get; set; }
        public int OpenedCount { get; set; }
        public int ClosedCount { get; set; }
    }
}
