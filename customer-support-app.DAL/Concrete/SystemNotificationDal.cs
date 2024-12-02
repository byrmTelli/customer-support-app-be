using customer_support_app.CORE.DataAccess.EntityFramework;
using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.RequestModels.SystemNotification;
using customer_support_app.CORE.ViewModels.SystemNotification;
using customer_support_app.DAL.Abstract;
using customer_support_app.DAL.Context.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.DAL.Concrete
{
    public class SystemNotificationDal : EfEntityRepositoryBase<SystemNotification, AppDbContext>, ISystemNotificationDal
    {
        private readonly AppDbContext _context;
        public SystemNotificationDal(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task CreateSystemNotification(CreateSystemNotificationRM model)
        {
            try
            {
                var systemNotification = new SystemNotification { Title = model.Title, Message = model.Message };

                await _context.AddAsync(systemNotification);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {

            }
        }
        public async Task<List<SystemNotificationVM>> GetAllSystemNotificationsAsync()
        {
            try
            {
                var allSystemNotificationsQuery = from notification in _context.SystemNotifications
                                                  select new SystemNotificationVM
                                                  {
                                                      Id = notification.Id,
                                                      Title = notification.Title,
                                                      Message = notification.Message,
                                                      CreatedAt = notification.CreatedAt
                                                  };

                var allSystemNotificationList = await allSystemNotificationsQuery.ToListAsync();



                return allSystemNotificationList;

            }
            catch (Exception ex)
            {
                // Logs will be here.
                return null;
            }
        }
    }
}
