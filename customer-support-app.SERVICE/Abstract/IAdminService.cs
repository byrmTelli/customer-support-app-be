using customer_support_app.CORE.RequestModels.Admin.Role;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.ViewModels.Admin.CategoriesPage;
using customer_support_app.CORE.ViewModels.Admin.Dashboard;
using customer_support_app.CORE.ViewModels.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.SERVICE.Abstract
{
    public interface IAdminService
    {
        Task<IDataResult<DashboardViewModel>> GetDashoardStatistics();
        Task<IDataResult<CategoriesPageViewModel>> GetCategoriesPageStatistics(int categoryId);
        Task<IDataResult<List<AssignRoleViewModel>>> GetRoles();
        Task<IResult> AssignRoleToUser(AssignRoleToUserRM model);
    }
}
