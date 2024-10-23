using AutoMapper;
using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.RequestModels.User;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.Results.Concrete;
using customer_support_app.CORE.ViewModels.User;
using customer_support_app.SERVICE.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using IResult = customer_support_app.CORE.Results.Abstract.IResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using customer_support_app.DAL.Abstract;

namespace customer_support_app.SERVICE.Concrete
{
    public class UserService:IUserService
    {
        private readonly IUserDal _userDal;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;
        public UserService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager,IMapper mapper,IUserDal userDal)
        {
            _userDal = userDal;
            _userManager = userManager;
            _roleManager = roleManager; 
            _mapper = mapper;
        }
        public async Task<IDataResult<List<CustomerProfileViewModel>>> GetCustomersForAdminPanelAsync()
        {
            try
            {
                var result = await _userDal.GetCustomersForAdminPanelAsync();

                return result;
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<CustomerProfileViewModel>>("Something went wrong. Please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<List<UserProfileViewModel>>> GetHelpDesksForAdminPanelAsync()
        {
            try
            {
                var result = await _userDal.GetHelpDesksForAdminPanelAsync();

                return result;
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<UserProfileViewModel>>("Something went wrong. Please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<List<HelpdeskViewModel>>> GetHelpdesksAsync()
        {
            try
            {
                var result = await _userDal.GetHelpdesksAsync();

                return result;
            }
            catch(Exception ex)
            {
                return new ErrorDataResult<List<HelpdeskViewModel>>("Something went wrong. Please check the application logs.",StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<UserProfileViewModel>> UpdateUserAsync(UpdateUserRequestModel model)

        {
            try
            {
                var isUserExist = await _userManager.FindByEmailAsync(model.Email);
                if(isUserExist == null)
                {
                    return new ErrorDataResult<UserProfileViewModel>("Bad request.", StatusCodes.Status400BadRequest);
                }

                isUserExist.UserName = model.Username;
                isUserExist.Name = model.Name;
                isUserExist.Surname = model.Surname;
                isUserExist.Adress = model.Address;
                isUserExist.PhoneNumber = model.PhoneNumber;

                var result = await _userManager.UpdateAsync(isUserExist);
                if(!result.Succeeded)
                {
                    return new ErrorDataResult<UserProfileViewModel>("Error occured.", StatusCodes.Status500InternalServerError);
                }

                var roleList = await _userManager.GetRolesAsync(isUserExist);
                var role = roleList.First();

                var userProfileViewModel = _mapper.Map<UserProfileViewModel>(isUserExist);

                userProfileViewModel.Role = new CORE.ViewModels.Role.RoleViewModel { Name = role};

                return new SuccessDataResult<UserProfileViewModel>(userProfileViewModel, StatusCodes.Status200OK);

            }
            catch(Exception ex)
            {
                return new ErrorDataResult<UserProfileViewModel>("Something went wrong. Please check the application logs.", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IResult> RegisterUserAsync(RegisterUserRequestModel model)
        {
            try
            {
                var newUser = _mapper.Map<AppUser>(model);
                var result = await _userManager.CreateAsync(newUser, model.Password);

                if(!result.Succeeded)
                {
                    return new ErrorResult("Error occured while creating user",StatusCodes.Status500InternalServerError);
                }

                var assignRoleResult = await _userManager.AddToRoleAsync(newUser,"customer");

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
