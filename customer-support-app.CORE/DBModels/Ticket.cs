using customer_support_app.CORE.Constants;
using customer_support_app.CORE.DBModels.BaseModels;
using customer_support_app.CORE.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.DBModels
{
    public class Ticket:BaseEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Status { get; set; } = TicketStatus.Pending;
        public int? AssignedUserId { get; set; }
        public AppUser? AssignedTo { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public bool IsPublished { get; set; } = false;
        public int CreatorId { get; set; }
        public AppUser Creator { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<ActivityLog> Activities { get; set; }
        public virtual ICollection<TicketNotification> Notifications { get; set; }
    }
}
