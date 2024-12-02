using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.DBModels
{
    public class SystemNotification
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Title { get; set; } = $"System Notification - {DateTime.Now}";
        public string Message { get; set; }
        public int CreatorId { get; set; }
    }
}
