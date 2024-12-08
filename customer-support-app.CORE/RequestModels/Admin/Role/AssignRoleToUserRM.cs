using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.RequestModels.Admin.Role
{
    public class AssignRoleToUserRM
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}
