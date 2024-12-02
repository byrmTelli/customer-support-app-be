using customer_support_app.CORE.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.RequestModels.TicketNotification
{
    public class CreateTicketNotificationRM
    {
        public string Title { get; set; }
        public string Message { get; set; }


    }
}
