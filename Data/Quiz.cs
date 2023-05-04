using MyCompany.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompany.Data
{
    public class Quiz
    {
        public int Id { get; set; }

        [Required]
        public string OwnerName { get; set; }

        [Required]
        public string QuizLink { get; set; }

        [Required]
        public Difficulty Level { get; set; }
    }
}
