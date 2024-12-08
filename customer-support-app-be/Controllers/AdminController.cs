using customer_support_app.CORE.RequestModels.Admin.Role;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.ViewModels.Admin.CategoriesPage;
using customer_support_app.CORE.ViewModels.Admin.Dashboard;
using customer_support_app.CORE.ViewModels.Role;
using customer_support_app.SERVICE.Abstract;
using Microsoft.AspNetCore.Mvc;
using IResult = customer_support_app.CORE.Results.Abstract.IResult;

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

        [HttpGet(nameof(GetRoles))]
        [ProducesResponseType(typeof(IDataResult<List<AssignRoleViewModel>>), 200)]
        [ProducesResponseType(typeof(IDataResult<List<AssignRoleViewModel>>), 400)]
        [ProducesResponseType(typeof(IDataResult<List<AssignRoleViewModel>>), 500)]
        public async Task<IActionResult> GetRoles()
        {
            var response = await _adminService.GetRoles();

            return StatusCode(response.Code, response);
        }

        [HttpPost(nameof(AssignRoleToUser))]
        [ProducesResponseType(typeof(IResult), 200)]
        [ProducesResponseType(typeof(IResult), 400)]
        [ProducesResponseType(typeof(IResult), 500)]
        public async Task<IActionResult> AssignRoleToUser([FromQuery] AssignRoleToUserRM request)
        {
            var response = await _adminService.AssignRoleToUser(request);

            return StatusCode(response.Code, response);
        }

        [HttpGet(nameof(GetCategoriesPageStatistics))]
        [ProducesResponseType(typeof(IDataResult<CategoriesPageViewModel>), 200)]
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
