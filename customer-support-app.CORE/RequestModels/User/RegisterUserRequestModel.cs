using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.RequestModels.User
{
    public class RegisterUserRequestModel
    {
        [Required(ErrorMessage ="Username field is required.")]
        [StringLength(30,MinimumLength =3,ErrorMessage ="Username lenght must be between 3 - 30 character.")]
        public string Username { get; set; }

        [Required(ErrorMessage ="Name field is required.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Name lenght must be between 3 - 30 character.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Username field is required.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Surname lenght must be between 3 - 30 character.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Email field is required.")]
        [EmailAddress(ErrorMessage ="Invalid email adress.")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Password field is required.")]
        [StringLength(50,MinimumLength =6,ErrorMessage ="Password field length must be between 6 - 50 character.")]
        public string Password { get; set; }

        [Required(ErrorMessage ="Password Confirm field is required.")]
        [Compare("Password",ErrorMessage ="Passwords you given did not match.")]
        public string PasswordConfirm { get; set; }

        [Phone(ErrorMessage ="Invalid phone number.")]
        public string PhoneNumber { get; set; }
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Adress field length must be between 6 - 50 character.")]
        public string Address { get; set; }
    }
}
