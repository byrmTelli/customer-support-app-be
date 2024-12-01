using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.Results.Concrete;
using customer_support_app.CORE.ViewModels.Admin.CategoriesPage;
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

        public async Task<IDataResult<CategoriesPageViewModel>> GetCategoriesPageStatistics(int categoryId)
        {
            try
            {
                var tickets  = await _adminDal.GetCategoriesPageStatisticsAsync(categoryId);

                return new SuccessDataResult<CategoriesPageViewModel>(tickets, StatusCodes.Status200OK);
            }
            catch(Exception ex)
            {
                return new ErrorDataResult<CategoriesPageViewModel>("Something went wrong while feching data.", StatusCodes.Status500InternalServerError);
            }
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
