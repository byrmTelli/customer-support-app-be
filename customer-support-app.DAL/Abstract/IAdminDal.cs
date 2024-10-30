using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.ViewModels.Admin.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.DAL.Abstract
{
    public interface IAdminDal
    {
        Task<IDataResult<DashboardViewModel>> GetDashboardDataAsync();
    }
}
