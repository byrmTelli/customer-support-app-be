using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.ViewModels.Admin.Dashboard
{
    public class DashboardViewModel
    {
        public int UserCount { get; set; }
        public int CustomerCount { get; set; }
        public int TicketCount { get; set; }
        public List<MonthlySupportRequest> MonthlySupportRequests { get; set; } = new List<MonthlySupportRequest>();
    }
}
