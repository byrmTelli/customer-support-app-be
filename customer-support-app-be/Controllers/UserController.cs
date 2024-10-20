using customer_support_app.CORE.RequestModels.User;
using customer_support_app.CORE.Results.Concrete;
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
