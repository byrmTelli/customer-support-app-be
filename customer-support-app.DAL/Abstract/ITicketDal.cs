using customer_support_app.CORE.Constants;
using customer_support_app.CORE.DataAccess;
using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.RequestModels.Ticket;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.ViewModels.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.DAL.Abstract
{
    public interface ITicketDal:IEntityRepository<Ticket>
    {
        Task<IResult> CreateTicketAsync(CreateTicketRequestModel model);
        Task<IDataResult<List<AdminPanelTicketsTableViewModel>>> GetAllTicketsForAdmin();
        Task<IDataResult<List<Ticket>>> GetTicketsOfUser(int id);
        Task<IDataResult<TicketViewModel>> GetTickedById(int ticketId, string senderId , string userRole);
        Task<IDataResult<Ticket>> UpdateTicket(UpdateTicketRequestModel model);
        Task<IResult> AssingTicketToHelpdeskAsync(int ticketId, string assignToUserId);
        Task<IDataResult<List<HelpdeskTicketsTableViewModel>>> GetTicketsOfHelpdesk(int id);
        Task<IResult> UpdateTicketStatus(string status, int ticketId,int userId,string userRole);
    }
}
