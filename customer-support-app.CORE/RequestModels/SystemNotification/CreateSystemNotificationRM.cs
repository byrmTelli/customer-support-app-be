using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.RequestModels.SystemNotification
{
    public class CreateSystemNotificationRM
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public int CreatorId { get; set; }

    }
}
