using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.RequestModels.Comment
{
    public class AddCommentToTicketRequestModel
    {
        public int TicketId { get; set; }
        public string Message { get; set; }
        public int CreatorId { get; set; }
    }
}
