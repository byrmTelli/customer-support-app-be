using customer_support_app.CORE.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.ViewModels.Comment
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public UserViewModel Creator { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
