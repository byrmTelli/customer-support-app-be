using AutoMapper;
using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.RequestModels.User;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.Results.Concrete;
using customer_support_app.CORE.Utilities;
using customer_support_app.CORE.ViewModels.User;
using customer_support_app.SERVICE.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.SERVICE.Concrete
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IConfiguration configuration,IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mapper = mapper;
        }
        public async Task<IDataResult<UserProfileViewModel>> GetUserProfileAsync(string id)
        {
            try
            {
                var isUserExist = await _userManager.FindByIdAsync(id);
                if(isUserExist == null )
                {
                    return new ErrorDataResult<UserProfileViewModel>("Bad request.",StatusCodes.Status400BadRequest);
                }


                var userRole = await _userManager.GetRolesAsync(isUserExist);
                var role = userRole.First(); 

                var userProfileVM = _mapper.Map<UserProfileViewModel>(isUserExist);
                userProfileVM.Role = new CORE.ViewModels.Role.RoleViewModel { Name = role };

                if(isUserExist.ProfileImage != null)
                {
                    string userProfileImage = ImageHelper.ConvertImageToBase64String(isUserExist.ProfileImage);
                    userProfileVM.ProfileImage = userProfileImage;
                }


                return new SuccessDataResult<UserProfileViewModel>(userProfileVM, StatusCodes.Status200OK);
            }
            catch (Exception)
            {
                return new ErrorDataResult<UserProfileViewModel>("Something went wrong. Please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<UserLoginViewModel>> LoginAsync(UserLoginRequestModel model)
        {
            try
            {
                var isUserExist = await _userManager.FindByNameAsync(model.Username);
                if (isUserExist == null)
                {
                    return new ErrorDataResult<UserLoginViewModel>("Username or password wrong.", StatusCodes.Status401Unauthorized);
                }

                var loginStatus = await _userManager.CheckPasswordAsync(isUserExist, model.Password);
                if (!loginStatus)
                {
                    return new ErrorDataResult<UserLoginViewModel>("Username or password wrong.", StatusCodes.Status401Unauthorized);
                }

                var jwtToken = await GenerateTokenAsync(isUserExist);
                var userLoginVM = new UserLoginViewModel { Token = jwtToken };

                return new SuccessDataResult<UserLoginViewModel>(userLoginVM,"Login successfully.", StatusCodes.Status200OK);
            }
            catch (Exception)
            {
                return new ErrorDataResult<UserLoginViewModel>("Something went wrong. Please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<string> GenerateTokenAsync(AppUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var role = userRoles.First();

            var authClaims = new List<Claim>
        {
            new Claim("UserID", user.Id.ToString()),
            new Claim("Username", user.UserName),
            new Claim("Email", user.Email),
            new Claim("Role", role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var tokenExpireTime = Convert.ToInt32(_configuration["Jwt:TokenExpireHour"]);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddHours(tokenExpireTime),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
