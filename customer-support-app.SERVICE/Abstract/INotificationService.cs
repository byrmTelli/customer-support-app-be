using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.Results.Abstract;

namespace customer_support_app.SERVICE.Abstract
{
    public interface INotificationService
    {
        Task<IDataResult<TicketNotification>> MarkAsReadAsync(int notificationId);
        Task<IResult> AddCommentToTicketNotificationAsync(int ticketId, string message);
    }

}