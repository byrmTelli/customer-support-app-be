using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.RequestModels.User
{
    public class UserLoginRequestModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
