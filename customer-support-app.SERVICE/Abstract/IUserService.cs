﻿using customer_support_app.CORE.RequestModels.User;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.SERVICE.Abstract
{
    public interface IUserService
    {
        Task<IResult> ApproveUser(int userId);
        Task<IDataResult<UserProfileForAdminPanelViewModel>> GetUserProfileForAdminPanelAsync(int id);
        Task<IDataResult<List<CustomerProfileViewModel>>> GetCustomersForAdminPanelAsync();
        Task<IDataResult<List<UserProfileViewModel>>> GetHelpDesksForAdminPanelAsync();
        Task<IDataResult<List<HelpdeskViewModel>>> GetHelpdesksAsync();
        Task<IResult> RegisterUserAsync(RegisterUserRequestModel model);
        Task<IResult> ResetPasswordAsync(ResetPasswordRequestModel model);
        Task<IResult> ForgotPasswordAsync(ForgotPasswordRequestModel model);
        // Change this return type as IResult after implementing send mail functionality.
        Task<string> SendPasswordResetEmailAsync(string emailAdress);
        Task<IDataResult<UserProfileViewModel>> UpdateUserAsync(UpdateUserRequestModel model);
    }
}
