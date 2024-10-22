using customer_support_app.CORE.ViewModels.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.ViewModels.User
{
    public class HelpdeskViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public RoleViewModel Role { get; set; }
    }
}
