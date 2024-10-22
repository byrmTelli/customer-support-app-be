using customer_support_app.CORE.DataAccess.EntityFramework;
using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.RequestModels.Ticket;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.Results.Concrete;
using customer_support_app.CORE.ViewModels.Ticket;
using customer_support_app.DAL.Abstract;
using customer_support_app.DAL.Context.DbContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.DAL.Concrete
{
    public class TicketDal : EfEntityRepositoryBase<Ticket, AppDbContext>, ITicketDal
    {
        private readonly AppDbContext _context;
        public TicketDal(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IDataResult<List<Ticket>>> GetTicketsOfHelpdesk(int userId)
        {
            try
            {
                var tickets = await _context.Tickets
                    .Include(t => t.Creator)
                    .Include(t => t.Category)
                    .Include(x => x.Activities)
                    .Include(t => t.Comments)
                    .ThenInclude(c => c.Creator)
                    .Where(t => t.AssignedUserId == userId)
                    .ToListAsync();

                return new SuccessDataResult<List<Ticket>>(tickets, StatusCodes.Status200OK);
            }
            catch(Exception ex)
            {
                return new ErrorDataResult<List<Ticket>>("Something went wrong while fetching data.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IResult> AssingTicketToHelpdeskAsync(int ticketId, string assignToUserId)
        {
            try
            {
                var isTicketExist = await _context.Tickets
                    .Include(x => x.Creator)
                    .Include(x => x.AssignedTo)
                    .Where(x => x.Id == ticketId)
                    .FirstOrDefaultAsync();


                if(isTicketExist == null)
                {
                    return new ErrorResult("Bad request.",StatusCodes.Status400BadRequest);
                }

                var userId = Int32.Parse(assignToUserId);

                isTicketExist.AssignedUserId = userId;
                await _context.SaveChangesAsync();

                return new SuccessResult("Assigned successfully.",StatusCodes.Status200OK);

            }
            catch(Exception ex)
            {
                return new ErrorResult("Something went wrong. Please check the application logs.",StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<Ticket>> GetTickedById(int ticketId, string senderId, string userRole)
        {
            try
            {

                var senderIdInt = Int32.Parse(senderId);

                var isTicketExist = await _context.Tickets
                    .Include(x => x.Creator)
                    .Include(x => x.Category)
                    .Include(x => x.AssignedTo)
                    .Include(x => x.Activities)
                    .Include(x => x.Comments)
                    .ThenInclude(c => c.Creator)
                    .Where(x => x.Id == ticketId)
                    .FirstOrDefaultAsync();


                if(userRole == "customer" && isTicketExist.CreatorId != senderIdInt)
                {
                    return new ErrorDataResult<Ticket>("Bad request.", StatusCodes.Status400BadRequest);
                }

                if(userRole == "helpdesk" && isTicketExist.AssignedUserId != senderIdInt)
                {
                    return new ErrorDataResult<Ticket>("Bad request.", StatusCodes.Status400BadRequest);
                }


                if (isTicketExist == null)
                {
                    return new ErrorDataResult<Ticket>("Bad request.", StatusCodes.Status400BadRequest);
                }


                return new SuccessDataResult<Ticket>(isTicketExist, StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<Ticket>("Something went wrong. Please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<List<Ticket>>> GetTicketsOfUser(int id)
        {
            try
            {

                var tickets = await _context.Tickets
                    .Include(t => t.Creator)
                    .Include(t => t.Category)
                    .Include(x => x.Activities)
                    .Include(t => t.Comments)
                    .ThenInclude(c => c.Creator)
                    .Where(t => t.CreatorId == id)
                    .ToListAsync();

                return new SuccessDataResult<List<Ticket>>(tickets, StatusCodes.Status200OK);


            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<Ticket>>("Something went wrong while fetching data.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<Ticket>> UpdateTicket(UpdateTicketRequestModel model)
        {
            try
            {
                var isTicketExist = await _context.Tickets
                    .Include(x => x.Creator)
                    .Include(x => x.Category)
                    .Where(x => x.IsDeleted == false && x.Id == model.Id)
                    .FirstOrDefaultAsync();

                if (isTicketExist == null)
                {
                    return new ErrorDataResult<Ticket>("Bad request.", StatusCodes.Status400BadRequest);
                }

                var isCategoryExist = await _context.Categories.Where(x => x.Id == model.CategoryId).FirstOrDefaultAsync();

                if (isCategoryExist == null)
                {
                    return new ErrorDataResult<Ticket>("Bad request.", StatusCodes.Status400BadRequest);
                }


                isTicketExist.Title = model.Title;
                isTicketExist.Content = model.Content;
                // Add UpdatedBy
                isTicketExist.UpdatedAt = DateTime.Now;
                isTicketExist.CategoryId = model.CategoryId;

                _context.Update(isTicketExist);
                await _context.SaveChangesAsync();

                return new SuccessDataResult<Ticket>(isTicketExist, StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return new ErrorDataResult<Ticket>("Something went wrong.", StatusCodes.Status500InternalServerError);
            }
        }
    }
}
