using customer_support_app.CORE.DataAccess.EntityFramework;
using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.Enums;
using customer_support_app.CORE.RequestModels.TicketNotification;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.Results.Concrete;
using customer_support_app.CORE.ViewModels.TicketNotification;
using customer_support_app.DAL.Abstract;
using customer_support_app.DAL.Context.DbContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.DAL.Concrete
{
    public class TicketNotificationDal : EfEntityRepositoryBase<TicketNotification, AppDbContext>, ITicketNotificationDal
    {
        private readonly AppDbContext _context;
        public TicketNotificationDal(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task CreateTicketNotificationAsync(CreateTicketNotificationRM model)
        {
            try
            {
                string title = "";
                string message = "";

                switch (model.NotificationType)
                {
                    case (TicketNotificationType.Completed):
                        title = "Ticket status has changed.";
                        message = $"Ticket-{model.TicketId} status has been updated as COMPLETED.";
                        break;
                    case (TicketNotificationType.Waiting):
                        title = "Ticket status has changed.";
                        message = $"Ticket-{model.TicketId} status has been updated as WAITING.";
                        break;
                    default:
                        title = "Ticket status has changed.";
                        message = $"Ticket-{model.TicketId} status has been updated as CANCELLED.";
                        break;
                }

                var newTicketNotification = new TicketNotification { Title = title, Content = message, TicketId = model.TicketId };

                await _context.AddAsync(newTicketNotification);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

            }
        }
        public async Task<List<TicketNotificationVM>> GetAllTicketNotificationsOfUser(int userId)
        {
            var usersTicketNotificationQuery = from notification in _context.TicketNotifications
                                          join ticket in _context.Tickets on notification.TicketId equals ticket.Id
                                          where ticket.CreatorId == userId
                                          select new TicketNotificationVM
                                          {
                                              Id = notification.Id,
                                              Title = notification.Title,
                                              Message = notification.Content,
                                              CreatedAt = notification.CreatedAt,
                                              TicketId = ticket.Id,
                                          };

            var usersTicketNotificationList = await usersTicketNotificationQuery.ToListAsync();

            return usersTicketNotificationList;
        }
        public async Task<List<TicketNotificationVM>> GetAllTicketNotificationsAsync()
        {

            var ticketNotificationsQuery = from notification in _context.TicketNotifications
                                           select new TicketNotificationVM
                                           {
                                               Title = notification.Title,
                                               Message = notification.Content,
                                               CreatedAt = notification.CreatedAt,
                                               TicketId = notification.TicketId
                                           };

            var ticketNotificationsList = await ticketNotificationsQuery.ToListAsync();

            return ticketNotificationsList;

        }
    }
}
