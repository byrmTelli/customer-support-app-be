using customer_support_app.API.Services.Auth;
using customer_support_app.CORE.Constants;
using customer_support_app.CORE.RequestModels.User;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.Results.Concrete;
using customer_support_app.CORE.ViewModels.User;
using customer_support_app.SERVICE.Abstract;
using Microsoft.AspNetCore.Mvc;
using IResult = customer_support_app.CORE.Results.Abstract.IResult;

namespace customer_support_app.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [CustomAuthorization(RoleTypes.Admin)]
        [HttpPost(nameof(ApproveUser))]
        [ProducesResponseType(typeof(IResult),200)]
        [ProducesResponseType(typeof(IResult), 400)]
        [ProducesResponseType(typeof(IResult), 500)]
        public async Task<IActionResult> ApproveUser(int id)
        {
            var response = await _userService.ApproveUser(id);

            return StatusCode(response.Code, response);
        }
        [CustomAuthorization(RoleTypes.Admin,RoleTypes.Helpdesk)]
        [HttpGet(nameof(GetUserProfileForAdminPanel))]
        [ProducesResponseType(typeof(IDataResult<UserProfileForAdminPanelViewModel>), 200)]
        [ProducesResponseType(typeof(IDataResult<UserProfileForAdminPanelViewModel>), 400)]
        [ProducesResponseType(typeof(IDataResult<UserProfileForAdminPanelViewModel>), 500)]
        public async Task<IActionResult> GetUserProfileForAdminPanel(int id)
        {
            var response = await _userService.GetUserProfileForAdminPanelAsync(id);

            return StatusCode(response.Code, response);
        }
        [CustomAuthorization(RoleTypes.Admin)]
        [HttpGet(nameof(GetCustomerProfileListForAdminPanel))]
        [ProducesResponseType(typeof(IDataResult<List<CustomerProfileViewModel>>), 200)]
        [ProducesResponseType(typeof(IDataResult<List<CustomerProfileViewModel>>), 500)]
        public async Task<IActionResult> GetCustomerProfileListForAdminPanel()
        {
            var response = await _userService.GetCustomersForAdminPanelAsync();
            return StatusCode(response.Code, response);
        }
        [CustomAuthorization(RoleTypes.Admin)]
        [HttpGet(nameof(GetHeldesksProfileListForAdminPanel))]
        [ProducesResponseType(typeof(IDataResult<List<UserProfileViewModel>>), 200)]
        [ProducesResponseType(typeof(IDataResult<List<UserProfileViewModel>>), 500)]
        public async Task<IActionResult> GetHeldesksProfileListForAdminPanel()
        {
            var response = await _userService.GetHelpDesksForAdminPanelAsync();
            return StatusCode(response.Code, response);
        }
        [CustomAuthorization(RoleTypes.Admin)]
        [HttpGet(nameof(GetHelpdesks))]
        [ProducesResponseType(typeof(IDataResult<List<HelpdeskViewModel>>), 200)]
        [ProducesResponseType(typeof(IDataResult<List<HelpdeskViewModel>>), 400)]
        [ProducesResponseType(typeof(IDataResult<List<HelpdeskViewModel>>), 500)]
        public async Task<IActionResult> GetHelpdesks()
        {
            var response  = await _userService.GetHelpdesksAsync();

            return StatusCode(response.Code, response);
        }
        [CustomAuthorization(RoleTypes.Admin,RoleTypes.Customer,RoleTypes.Helpdesk)]
        [HttpPut(nameof(UpdateUser))]
        [ProducesResponseType(typeof(IResult), 200)]
        [ProducesResponseType(typeof(IResult), 400)]
        [ProducesResponseType(typeof(IResult), 500)]
        public async Task<IActionResult> UpdateUser(UpdateUserRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                var result = new ErrorResult("Validation failed.", StatusCodes.Status400BadRequest, errors);

                return StatusCode(result.Code, result);
            }

            var response = await _userService.UpdateUserAsync(request);

            return StatusCode(response.Code, response);

        }
        [HttpPost(nameof(Register))]
        [ProducesResponseType(typeof(IResult), 200)]
        [ProducesResponseType(typeof(IResult), 400)]
        [ProducesResponseType(typeof(IResult), 500)]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                var result = new ErrorResult("Validation failed.",StatusCodes.Status400BadRequest,errors);

                return StatusCode(result.Code, result);
            }


            var response = await _userService.RegisterUserAsync(request);

            return StatusCode(response.Code,response);
        }

        [HttpPost(nameof(ResetPassword))]
        [ProducesResponseType(typeof(IResult), 200)]
        [ProducesResponseType(typeof(IResult), 400)]
        [ProducesResponseType(typeof(IResult), 500)]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                var result = new ErrorResult("Validation failed.", StatusCodes.Status400BadRequest, errors);

                return StatusCode(result.Code, result);
            }

            var response = await _userService.ResetPasswordAsync(request);

            return StatusCode(response.Code, response);

        }
        [HttpPost(nameof(ForgotPassword))]
        [ProducesResponseType(typeof(IResult), 200)]
        [ProducesResponseType(typeof(IResult), 400)]
        [ProducesResponseType(typeof(IResult), 500)]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                var result = new ErrorResult("Validation failed.", StatusCodes.Status400BadRequest, errors);

                return StatusCode(result.Code, result);
            }

            var response = await _userService.ForgotPasswordAsync(request);

            return StatusCode(response.Code, response);

        }

        [HttpPost(nameof(SendPasswordResetEmail))]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> SendPasswordResetEmail(string emailAdress)
        {
            var result = await _userService.SendPasswordResetEmailAsync(emailAdress);
            // Change here after implement email sending.
            return Ok(result);
        }
    }
}
