using customer_support_app.CORE.RequestModels.TicketNotification;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.ViewModels.TicketNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.DAL.Abstract
{
    public interface ITicketNotificationDal
    {
        Task CreateTicketNotificationAsync(CreateTicketNotificationRM model);
        Task<List<TicketNotificationVM>> GetAllTicketNotificationsOfUser(int userId);
        Task<List<TicketNotificationVM>> GetAllTicketNotificationsAsync();
    }
}
