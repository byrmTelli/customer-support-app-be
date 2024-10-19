using AutoMapper;
using customer_support_app.CORE.RequestModels.Comment;
using customer_support_app.CORE.DBModels;
using customer_support_app.DAL.Abstract;
using customer_support_app.DAL.Concrete;
using customer_support_app.SERVICE.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IResult = customer_support_app.CORE.Results.Abstract.IResult;
using customer_support_app.CORE.Results.Concrete;
using Microsoft.AspNetCore.Http;
using customer_support_app.CORE.ViewModels.Comment;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.ViewModels.User;

namespace customer_support_app.SERVICE.Concrete
{
    public class CommentService:ICommentService
    {
        private readonly ICommentDal _commentDal;
        private readonly ITicketDal _ticketDal;
        private readonly IMapper _mapper;
        public CommentService(ICommentDal commentDal,IMapper mapper)
        {
            _commentDal = commentDal;
            _mapper = mapper;
        }

        public async Task<IResult> AddCommentToTicket(AddCommentToTicketRequestModel model)
        {
            try
            {
                var response = await _commentDal.AddCommentToTicket(_mapper.Map<Comment>(model));

                return new SuccessResult(response.Message,response.Code);
            }
            catch (Exception ex)
            {
                return new ErrorResult("Something went wrong.", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<IDataResult<CommentViewModel>> UpdateComment(UpdateCommentRequestModel model, string updatedBy)
        {
            try
            {
                var response = await _commentDal.UpdateComment(_mapper.Map<Comment>(model), updatedBy);

                return new SuccessDataResult<CommentViewModel>(_mapper.Map<CommentViewModel>(response.Data),StatusCodes.Status200OK);
            }
            catch (Exception ex) 
            {
                return new ErrorDataResult<CommentViewModel>("Something went wrong.",StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IResult> DeleteComment(int id)
        {
            try
            {
                var isCommentExist = await _commentDal.GetAsync(c => c.Id == id);

                if(isCommentExist == null)
                    return new ErrorResult("Bad Request.",StatusCodes.Status400BadRequest);

                await _commentDal.DeleteAsync(isCommentExist,"dummy",false);

                return new SuccessResult("Entity deleted successfully.",StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return new ErrorResult("Something went wrong. Please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        }
    }
}
