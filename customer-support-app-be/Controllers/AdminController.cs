using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.ViewModels.Admin.CategoriesPage;
using customer_support_app.CORE.ViewModels.Admin.Dashboard;
using customer_support_app.SERVICE.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace customer_support_app.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet(nameof(GetCategoriesPageStatistics))]
        [ProducesResponseType(typeof(IDataResult<CategoriesPageViewModel>),200)]
        public async Task<IActionResult> GetCategoriesPageStatistics(int categoryId)
        {
            var response = await _adminService.GetCategoriesPageStatistics(categoryId);
            return StatusCode(response.Code, response);
        }
        [HttpGet(nameof(GetDashboardStats))]
        [ProducesResponseType(typeof(IDataResult<DashboardViewModel>), 200)]
        public async Task<IActionResult> GetDashboardStats()
        {
            var response = await _adminService.GetDashoardStatistics();
            return StatusCode(response.Code, response);
        }

    }
}
