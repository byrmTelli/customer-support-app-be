using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.CORE.RequestModels.User
{
    public class UpdateUserRequestModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Username field is required.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Username lenght must be between 3 - 30 character.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Name field is required.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Name lenght must be between 3 - 30 character.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Username field is required.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Surname lenght must be between 3 - 30 character.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Email field is required.")]
        [EmailAddress(ErrorMessage = "Invalid email adress.")]
        public string Email { get; set; }

        public string? PhoneNumber { get; set; }
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Adress field length must be between 6 - 50 character.")]
        public string Address { get; set; }
        public string ProfileImage { get; set; }
    }
}
