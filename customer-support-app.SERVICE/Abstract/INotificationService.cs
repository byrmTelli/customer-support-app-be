using customer_support_app.CORE.RequestModels.SystemNotification;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.ViewModels.SystemNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.SERVICE.Abstract
{
    public interface INotificationService
    {
        Task<IDataResult<List<SystemNotificationVM>>> GetSystemNotificationsAsync();
        Task<IResult> CreateSystemNotification(CreateSystemNotificationRM model);
    }
}
