using customer_support_app.CORE.Enums;
using customer_support_app.CORE.ViewModels.Category;
using customer_support_app.CORE.ViewModels.User;
using customer_support_app.CORE.ViewModels.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.ViewModels.Ticket
{
    public class TicketViewModel
    {
        public  int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public TicketStatus Status { get; set; }
        public CategoryViewModel Category { get; set; }
        public UserViewModel Creator { get; set; }
        public List<CommentViewModel> Comments { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
