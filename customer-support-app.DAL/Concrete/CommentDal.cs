using customer_support_app.CORE.DataAccess.EntityFramework;
using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.RequestModels.Comment;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.Results.Concrete;
using customer_support_app.DAL.Abstract;
using customer_support_app.DAL.Context.DbContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.DAL.Concrete
{
    public class CommentDal:EfEntityRepositoryBase<Comment,AppDbContext>,ICommentDal
    {
        private readonly AppDbContext _context;
        public CommentDal(AppDbContext context):base(context)
        {
            _context = context;
        }

        public async Task<IResult> AddCommentToTicket(Comment entity)
        {
            try
            {
                var isTicketExist = await _context.Tickets.Where(ticket => ticket.Id == entity.TicketId).FirstOrDefaultAsync();
                
                if (isTicketExist == null)
                {
                    return new ErrorResult("Bad request.", StatusCodes.Status400BadRequest);
                }

                await _context.AddAsync(entity);
                await _context.SaveChangesAsync();

                return new SuccessResult("Entity created successfully.", StatusCodes.Status201Created);

            }
            catch (Exception ex)
            {
                return new ErrorResult("Something went wrong.Please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<IDataResult<Comment>> UpdateComment(Comment model, string updatedBy)
        {
            try
            {
                var isCommentExist = await _context.Comments
                    .Include(c => c.Creator)
                    .Where(c => c.Id == model.Id)
                    .FirstOrDefaultAsync();

                if(isCommentExist == null)
                {
                    return new ErrorDataResult<Comment>("Bad request.",StatusCodes.Status400BadRequest);
                }

                isCommentExist.Message = model.Message;
                isCommentExist.UpdatedBy = updatedBy;
                isCommentExist.UpdatedAt = DateTime.Now;

                _context.Update(isCommentExist);
                await _context.SaveChangesAsync();

                return new SuccessDataResult<Comment>(isCommentExist,StatusCodes.Status200OK);

            }
            catch (Exception ex) 
            {
                return new ErrorDataResult<Comment>("Something went wrong. Please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        }
    }
}
