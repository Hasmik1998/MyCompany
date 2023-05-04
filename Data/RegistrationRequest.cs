using MyCompany.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace MyCompany.Data
{
    public class RegistrationRequest
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string SurName { get; set; }

        [Required]
        public UserType UserType { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public String Password { get; set; }

        public string Image { get; set; }

        public string About { get; set; }

        public bool Accept { get; set; }
    }
}
