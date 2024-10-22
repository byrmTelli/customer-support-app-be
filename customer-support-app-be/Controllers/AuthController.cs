using customer_support_app.CORE.RequestModels.User;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.Results.Concrete;
using customer_support_app.CORE.ViewModels.User;
using customer_support_app.SERVICE.Abstract;
using customer_support_app.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using customer_support_app.API.Services.Auth;

namespace customer_support_app.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [CustomAuthorization("admin","helpdesk","customer")]
        [HttpGet(nameof(GetUserProfile))]
        [ProducesResponseType(typeof(IDataResult<UserProfileViewModel>), 200)]
        [ProducesResponseType(typeof(IDataResult<UserProfileViewModel>), 400)]
        [ProducesResponseType(typeof(IDataResult<UserProfileViewModel>), 500)]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = User.FindFirstValue("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                var result = new ErrorDataResult<UserProfileViewModel>("Bad request.",StatusCodes.Status400BadRequest);

                return StatusCode(result.Code,result);
            }

            var response = await _authService.GetUserProfileAsync(userId);

            return StatusCode(response.Code, response);

        }
        [HttpPost(nameof(Login))]
        [ProducesResponseType(typeof(IDataResult<UserLoginViewModel>), 200)]
        [ProducesResponseType(typeof(IDataResult<UserLoginViewModel>), 400)]
        [ProducesResponseType(typeof(IDataResult<UserLoginViewModel>), 500)]
        public async Task<IActionResult> Login(UserLoginRequestModel request) 
        {
            if(!ModelState.IsValid)
            {
                var result = new ErrorDataResult<UserLoginViewModel>("Bad request.", StatusCodes.Status400BadRequest);
                
                return StatusCode(result.Code, result);
            }

            var response = await _authService.LoginAsync(request);

            return StatusCode(response.Code, response);

        }
    }
}
