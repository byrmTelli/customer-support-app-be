using customer_support_app.CORE.RequestModels.SystemNotification;
using customer_support_app.CORE.RequestModels.TicketNotification;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.ViewModels.SystemNotification;
using customer_support_app.CORE.ViewModels.TicketNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IResult = customer_support_app.CORE.Results.Abstract.IResult;


namespace customer_support_app.SERVICE.Abstract
{
    public interface INotificationService
    {
        Task<IDataResult<List<SystemNotificationVM>>> GetSystemNotificationsAsync();
        Task<IDataResult<List<TicketNotificationVM>>> GetTicketNotificationsAsync();
        Task<IDataResult<List<TicketNotificationVM>>> GetTicketNotificationsOfUserAsync(int userId);
        Task<IResult> CreateSystemNotificationAsync(CreateSystemNotificationRM model);
        Task<IResult> CreateTicketNotificationAsync(CreateTicketNotificationRM model);
    }
}
