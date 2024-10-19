using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.RequestModels.Comment
{
    public class UpdateCommentRequestModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
    }
}
