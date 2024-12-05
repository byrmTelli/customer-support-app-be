using customer_support_app.CORE.Constants;
using customer_support_app.CORE.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.RequestModels.TicketNotification
{
    public class CreateTicketNotificationRM
    {
        public int TicketId { get; set; }
        public TicketNotificationType NotificationType { get; set; }

    }
}
