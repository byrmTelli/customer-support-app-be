using customer_support_app.CORE.DataAccess.EntityFramework;
using customer_support_app.CORE.DBModels;
using customer_support_app.DAL.Abstract;
using customer_support_app.DAL.Context.DbContext;

namespace customer_support_app.DAL.Concrete
{
    public class TicketNotificationDal:EfEntityRepositoryBase<TicketNotification,AppDbContext>,ITicketNotificationDal
    {
        private readonly AppDbContext _context;
        public TicketNotificationDal(AppDbContext context):base(context)
        {
            _context = context;
        }

    }
}