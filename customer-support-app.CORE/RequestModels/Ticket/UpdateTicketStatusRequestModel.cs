using customer_support_app.CORE.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.RequestModels.Ticket
{
    public class UpdateTicketStatusRequestModel
    {
        public int Id { get; set; }
        public string Status { get; set; }
    }
}
