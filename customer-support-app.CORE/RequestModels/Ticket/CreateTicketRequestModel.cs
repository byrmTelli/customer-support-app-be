using customer_support_app.CORE.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.RequestModels.Ticket
{
    public class CreateTicketRequestModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int CategoryId { get; set; }
        public int CreatorId { get; set; }
        public List<IFormFile>? Files { get; set; }
    }
}
