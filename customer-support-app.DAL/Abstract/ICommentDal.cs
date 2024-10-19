using customer_support_app.CORE.DataAccess;
using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.RequestModels.Comment;
using customer_support_app.CORE.Results.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IResult = customer_support_app.CORE.Results.Abstract.IResult;


namespace customer_support_app.DAL.Abstract
{
    public interface ICommentDal:IEntityRepository<Comment>
    {
        Task<IResult> AddCommentToTicket(Comment model);
        Task<IDataResult<Comment>> UpdateComment(Comment model, string updatedBy);
    }
}
