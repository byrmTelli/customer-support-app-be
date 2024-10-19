using customer_support_app.CORE.DBModels.BaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.DBModels
{
    public class Comment:BaseEntity
    {
        public string Message { get; set; }
        public int CreatorId { get; set; }
        public AppUser Creator { get; set; }
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }
    }
}
