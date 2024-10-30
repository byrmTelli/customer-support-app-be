using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.Results.Concrete;
using customer_support_app.CORE.ViewModels.Admin.Dashboard;
using customer_support_app.DAL.Abstract;
using customer_support_app.SERVICE.Abstract;
using Microsoft.AspNetCore.Http;

namespace customer_support_app.SERVICE.Concrete
{
    public class AdminService : IAdminService
    {
        private readonly IAdminDal _adminDal;
        public AdminService(IAdminDal adminDal)
        {
            _adminDal = adminDal;
        }
        public async Task<IDataResult<DashboardViewModel>> GetDashoardStatistics()
        {
            try
            {
                var result = await _adminDal.GetDashboardDataAsync();

                return result;
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<DashboardViewModel>("Error occured while fetching data.",StatusCodes.Status500InternalServerError);
            }
        }
    }
}
