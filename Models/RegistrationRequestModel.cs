using Microsoft.AspNetCore.Http;
using MyCompany.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace MyCompany.Models
{
    public class RegistrationRequestModel
    {
        [Required(ErrorMessage = "This field is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public string SurName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public UserType UserType { get; set; }

        [Required]
        [RegularExpression(@"^([0-9]( |-)?)?(\(?[0-9]{3}\)?|[0-9]{3})( |-|/)?([0-9]{3}( |-)?[0-9]{4}(( x | ext | extension |  |-|/)(\d{3}))?)$", ErrorMessage = "Format should be xxx-xxx-xxxx")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,15}|[0-9]{1,15})(\]?)$", ErrorMessage = "The Email field is not a valid e-mail address.")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=[^\d_].*?\d)\w(\w|[!@#$%]){7,20}", ErrorMessage = @"Error. Password must have one capital, one special character and one numerical character. It can not start with a special character or a digit.")]
        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        public string Password { get; set; }

        public string UserImage { get; set; }

        [Display(Name = "Avatar")]
        public IFormFile Avatar { get; set; }

        public string About { get; set; }

        public bool Accept { get; set; }
    }
}
