using customer_support_app.CORE.Enums;
using customer_support_app.CORE.ViewModels.Category;
using customer_support_app.CORE.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.ViewModels.Ticket
{
    public class AdminPanelTicketsTableViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public CategoryViewModel Category { get; set; }
        public string Status { get; set; }
        public HelpdeskViewModel AssignedTo { get; set; }
        public CreatorViewModel Creator { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
