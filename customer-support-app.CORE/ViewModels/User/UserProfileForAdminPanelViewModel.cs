using customer_support_app.CORE.ViewModels.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.ViewModels.User
{
    public class UserProfileForAdminPanelViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Adress { get; set; }
        public DateTime CreatedAt { get; set; }
        public RoleViewModel Role { get; set; }
        public string ProfileImage { get; set; }
        public bool IsApproved { get; set; }
    }
}
