using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.DAL.Abstract;
using customer_support_app.DAL.Context.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.DAL.Concrete
{
    public class LogActivityDal : ILogActivityDal
    {
        private readonly AppDbContext _context;
        public LogActivityDal(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogActivity(ActivityLog activityLog)
        {
            await _context.AddAsync(activityLog);
            await _context.SaveChangesAsync();
        }
    }
}
