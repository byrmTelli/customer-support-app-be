using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.Results.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.DAL.Abstract
{
    public interface ILogActivityDal
    {
        Task LogActivity(ActivityLog activityLog);
    }
}
