using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.RequestModels.SystemNotification;
using customer_support_app.CORE.ViewModels.SystemNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.DAL.Abstract
{
    public interface ISystemNotificationDal
    {
        Task CreateSystemNotification(CreateSystemNotificationRM model);
        Task<List<SystemNotificationVM>> GetAllSystemNotificationsAsync();
    }
}
