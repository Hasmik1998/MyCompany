using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompany.Models
{
    public class TrinerRequestModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string SurName { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string UserImage { get; set; }

        [Required]
        [Display(Name = "Avatar")]
        public IFormFile Avatar { get; set; }

        [Required]
        public string About { get; set; }

        [Required]
        public string MyRequest { get; set; }

        public bool Accept { get; set; }

    }
}
