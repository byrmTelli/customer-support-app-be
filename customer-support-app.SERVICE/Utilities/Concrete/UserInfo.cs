using customer_support_app.SERVICE.Utilities.Abstract;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.SERVICE.Utilities.Concrete
{
    public class UserInfo : IUserInfo
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public UserInfo(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        public string GetUserEmail()
        {
            return _contextAccessor.HttpContext?.User?.FindFirst("Email").Value;
        }

        public string GetUserID()
        {
            return _contextAccessor.HttpContext?.User?.FindFirst("UserID").Value;
        }

        public string GetUserName()
        {
            return _contextAccessor.HttpContext?.User?.FindFirst("Username").Value;
        }

        public string GetUserRole()
        {
            return _contextAccessor.HttpContext?.User?.FindFirst("Role").Value;
        }
    }
}
