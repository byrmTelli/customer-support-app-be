using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.RequestModels.User;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.SERVICE.Abstract
{
    public interface IAuthService
    {
        Task<IDataResult<UserProfileViewModel>> GetUserProfileAsync(string id);
        Task<IDataResult<UserLoginViewModel>> LoginAsync(UserLoginRequestModel model);
        Task<string> GenerateTokenAsync(AppUser user);
    }
}
