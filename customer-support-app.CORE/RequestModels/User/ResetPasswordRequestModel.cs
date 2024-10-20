using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.RequestModels.User
{
    public class ResetPasswordRequestModel
    {
        public string Email { get; set; }
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "Password field is required.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password field length must be between 6 - 50 character.")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Password Confirm field is required.")]
        [Compare("NewPassword",ErrorMessage ="Passwords did not match.")]
        public string PasswordConfirm { get; set; }
    }
}
