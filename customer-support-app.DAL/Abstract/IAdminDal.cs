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
using IResult = customer_support_app.CORE.Results.Abstract.IResult;


namespace customer_support_app.DAL.Abstract
{
    public interface IAdminDal
    {
        Task<IDataResult<DashboardViewModel>> GetDashboardDataAsync();
        Task<IDataResult<List<AssignRoleViewModel>>> GetRolesAsyns();
        Task<CategoriesPageViewModel> GetCategoriesPageStatisticsAsync(int categoryId);
        Task<IResult> AssignRoleToUser(AssignRoleToUserRM model);
    }
}
