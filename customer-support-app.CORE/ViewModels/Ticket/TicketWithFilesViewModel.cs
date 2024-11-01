using customer_support_app.CORE.ViewModels.Category;
using customer_support_app.CORE.ViewModels.Comment;
using customer_support_app.CORE.ViewModels.LogActivity;
using customer_support_app.CORE.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.ViewModels.Ticket
{
    public class TicketWithFilesViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Status { get; set; }
        public HelpdeskViewModel AssignedTo { get; set; }
        public CategoryViewModel Category { get; set; }
        public UserViewModel Creator { get; set; }
        public List<CommentViewModel> Comments { get; set; }
        public List<LogActivityViewModel> Activities { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Files { get; set; }
    }
}
