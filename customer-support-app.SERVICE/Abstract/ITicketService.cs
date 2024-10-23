using customer_support_app.CORE.RequestModels.Comment;
using customer_support_app.CORE.RequestModels.Ticket;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.ViewModels.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.SERVICE.Abstract
{
    public interface ITicketService
    {
        Task<IDataResult<List<AdminPanelTicketsTableViewModel>>> GetAllTicketForAdmin();
        Task<IResult> AssingTicketToHelpdeskAsync(int ticketId, string assignToUserId);
        Task<IDataResult<List<TicketViewModel>>> GetTicketsOfUser(int id);
        Task<IResult> CreateTicket(CreateTicketRequestModel model);
        Task<IDataResult<TicketViewModel>> UpdateTicket(UpdateTicketRequestModel model);
        Task<IDataResult<TicketViewModel>> GetTicketById(int ticketId,string senderId);
        Task<IResult> DeleteTicket(int id);
    }
}
