using MyCompany.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompany.Models
{
    public class QuizViewModel
    {
        [Required]
        public string OwnerName { get; set; }

        [Required]
        public string QuizLink { get; set; }

        [Required]
        public Difficulty Level { get; set; }
    }
}
