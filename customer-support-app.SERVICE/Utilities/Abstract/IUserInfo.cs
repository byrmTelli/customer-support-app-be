using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.SERVICE.Utilities.Abstract
{
    public interface IUserInfo
    {
        string GetUserName();
        string GetUserID();
        string GetUserEmail();
        string GetUserRole();
    }
}
