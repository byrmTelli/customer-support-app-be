using customer_support_app.CORE.DataAccess.EntityFramework;
using customer_support_app.CORE.DBModels;
using customer_support_app.DAL.Abstract;
using customer_support_app.DAL.Context.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.DAL.Concrete
{
    public class TicketNotificationDal:EfEntityRepositoryBase<TicketNotification,AppDbContext>,ITicketNotificationDal
    {
        private readonly AppDbContext _context;
        public TicketNotificationDal(AppDbContext context):base(context)
        {
            _context = context;
        }

        public async Task CreateTicketNotificationAsync(int ticketId, string message)
        {
            try
            {
                var newTicketNotification = new TicketNotification { Title =  };
            }
            catch(Exception ex)
            {

            }
        }
    }
}
