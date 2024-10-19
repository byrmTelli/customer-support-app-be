using customer_support_app.CORE.RequestModels.Comment;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.ViewModels.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IResult = customer_support_app.CORE.Results.Abstract.IResult;
namespace customer_support_app.SERVICE.Abstract
{
    public interface ICommentService
    {
        Task<IResult> AddCommentToTicket(AddCommentToTicketRequestModel model);
        Task<IDataResult<CommentViewModel>> UpdateComment(UpdateCommentRequestModel model,string updatedBy);
    }
}
