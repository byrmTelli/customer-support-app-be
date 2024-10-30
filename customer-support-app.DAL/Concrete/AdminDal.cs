using customer_support_app.CORE.Constants;
using customer_support_app.CORE.DataAccess.EntityFramework;
using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.ViewModels.Admin.Dashboard;
using customer_support_app.DAL.Abstract;
using customer_support_app.DAL.Context.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using customer_support_app.CORE.Results.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace customer_support_app.DAL.Concrete
{
    public class AdminDal : EfEntityRepositoryBase<AppUser, AppDbContext>, IAdminDal
    {
        private readonly AppDbContext _context;
        public AdminDal(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IDataResult<DashboardViewModel>> GetDashboardDataAsync()
        {
            try
            {
                var usersQuery = from userRole in _context.UserRoles
                                 join user in _context.Users on userRole.UserId equals user.Id
                                 join role in _context.Roles on userRole.RoleId equals role.Id
                                 where role.Name != RoleTypes.Customer
                                 select user;

                var usersCount = usersQuery.Count();


                var customersQuery = from userRole in _context.UserRoles
                                 join user in _context.Users on userRole.UserId equals user.Id
                                 join role in _context.Roles on userRole.RoleId equals role.Id
                                 where role.Name == RoleTypes.Customer
                                 select user;

                var customersCount = customersQuery.Count();

                var ticketsQuery = from ticket in _context.Tickets select ticket;

                var ticketCount = ticketsQuery.Count();

                // Yılın tüm aylarını içeren bir liste oluşturuyoruz
                var allMonths = Enumerable.Range(1, 12).Select(month => new MonthlySupportRequest
                {
                    Month = $"{DateTime.Now.Year}-{month:D2}",
                    OpenedCount = 0,
                    ClosedCount = 0
                }).ToList();

                // Açılan taleplerin (OpenedCount) sorgusu
                var openedTicketsQuery = from ticket in _context.Tickets
                                         group ticket by new { Month = ticket.CreatedAt.Month, Year = ticket.CreatedAt.Year } into t
                                         select new
                                         {
                                             Month = $"{t.Key.Year}-{t.Key.Month:D2}",
                                             OpenedCount = t.Count()
                                         };

                var openedTickets = await openedTicketsQuery.ToListAsync();

                // Kapanan taleplerin (ClosedCount) sorgusu
                var closedTicketsQuery = from ticket in _context.Tickets
                                         where ticket.ClosedAt.HasValue
                                         group ticket by new { Month = ticket.ClosedAt.Value.Month, Year = ticket.ClosedAt.Value.Year } into t
                                         select new
                                         {
                                             Month = $"{t.Key.Year}-{t.Key.Month:D2}",
                                             ClosedCount = t.Count()
                                         };

                var closedTickets = await closedTicketsQuery.ToListAsync();

                // Açılan ve kapanan talepleri tüm aylar listesi ile birleştiriyoruz
                var completeMonthlyData = allMonths
                    .GroupJoin(openedTickets,
                               all => all.Month,
                               opened => opened.Month,
                               (all, opened) => new MonthlySupportRequest
                               {
                                   Month = all.Month,
                                   OpenedCount = opened.FirstOrDefault()?.OpenedCount ?? 0,
                                   ClosedCount = 0 // Bu değer bir sonraki birleşimde güncellenecek
                               })
                    .GroupJoin(closedTickets,
                               all => all.Month,
                               closed => closed.Month,
                               (all, closed) => new MonthlySupportRequest
                               {
                                   Month = all.Month,
                                   OpenedCount = all.OpenedCount,
                                   ClosedCount = closed.FirstOrDefault()?.ClosedCount ?? 0
                               })
                    .ToList();


                var dashboardViewModel = new DashboardViewModel
                {
                    UserCount = usersCount,
                    CustomerCount = customersCount,
                    TicketCount = ticketCount,
                    MonthlySupportRequests = completeMonthlyData,
                };



                return new SuccessDataResult<DashboardViewModel>(dashboardViewModel, StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<DashboardViewModel>("", StatusCodes.Status500InternalServerError);
            }
        }
    }
}
