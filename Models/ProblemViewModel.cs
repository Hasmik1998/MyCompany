using Microsoft.AspNetCore.Http;
using MyCompany.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompany.Models
{
    public class ProblemViewModel
    {
        [Required]
        [Display(Name = "Avatar")]
        public IFormFile Avatar { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Solution  { get; set; }
        
        [Required]
        public Difficulty Level { get; set; }
    }
}
