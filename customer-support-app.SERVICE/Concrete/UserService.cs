using AutoMapper;
using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.RequestModels.User;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.Results.Concrete;
using customer_support_app.SERVICE.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.SERVICE.Concrete
{
    public class UserService:IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;
        public UserService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager,IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager; 
            _mapper = mapper;
        }

        public async Task<IResult> RegisterUserAsync(RegisterUserRequestModel model)
        {
            try
            {
                var result = await _userManager.CreateAsync(_mapper.Map<AppUser>(model), model.Password);

                if(!result.Succeeded)
                {
                    return new ErrorResult("Error occured while creating user",StatusCodes.Status500InternalServerError);
                }

                return new SuccessResult("User created successfully.",StatusCodes.Status201Created);
            }
            catch(Exception ex)
            {
                return new ErrorResult("Something went wrong. Please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<IResult> ResetPasswordAsync(ResetPasswordRequestModel model)
        {
            try
            {
                var isUserExist = await _userManager.FindByEmailAsync(model.Email);
                if (isUserExist == null)
                {
                    return new ErrorResult("Bad request.", StatusCodes.Status400BadRequest);
                }

                var resetPaswordToken = await _userManager.GeneratePasswordResetTokenAsync(isUserExist);

                var result = await _userManager.ResetPasswordAsync(isUserExist, resetPaswordToken,model.NewPassword);

                if (!result.Succeeded)
                {
                    return new ErrorResult("Error occured while changing password.", StatusCodes.Status500InternalServerError);
                }

                return new SuccessResult("Pasword changed successfully.",StatusCodes.Status200OK);

            }
            catch(Exception ex)
            {
                return new ErrorResult("Something went wrong. Please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IResult> ForgotPasswordAsync(ForgotPasswordRequestModel model)
        {
            try
            {
                var isUserExist = await _userManager.FindByEmailAsync(model.Email);
                if (isUserExist == null)
                {
                    return new ErrorResult("Bad request.", StatusCodes.Status400BadRequest);
                }

                var result = await _userManager.ResetPasswordAsync(isUserExist, model.PasswordResetToken, model.Password);

                if (!result.Succeeded)
                {
                    return new ErrorResult("Error occured while changing password.", StatusCodes.Status500InternalServerError);
                }

                return new SuccessResult("Pasword changed successfully.", StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return new ErrorResult("Something went wrong. Please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<string> SendPasswordResetEmailAsync(string emailAdress)
        {
            try
            {
                var isUserExist = await _userManager.FindByEmailAsync(emailAdress);
                // Check is user exist.

                // Change return type string => IResult

                var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(isUserExist);

                return resetPasswordToken;

            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
