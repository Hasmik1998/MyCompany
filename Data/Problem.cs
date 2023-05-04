using MyCompany.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompany.Data
{
    public class Problem
    {
        public int Id { get; set; }

        [Required]
        public string ProblemImage { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Solution { get; set; }
        
        [Required]
        public Difficulty Level { get; set; }
    }
}
