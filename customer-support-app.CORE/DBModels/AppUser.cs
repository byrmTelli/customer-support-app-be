using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.DBModels
{
    public class AppUser:IdentityUser<int>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Adress { get; set; }
        public List<Ticket> UsersTickets { get; set; }
        public List<Ticket> AssignedTickets { get; set; }
        public List<Comment> Comments { get; set; }
        public List<ActivityLog> TicketActivities { get; set; }
        public List<TicketNotification> TicketNotifications { get; set; }
        public DateTime CreatedAt { get; set; }=DateTime.Now;
        public DateTime? DeletedAt { get; set; }
        public bool IsApproved { get; set; } = false;
        public byte[]? ProfileImage { get; set; }
    }
}
