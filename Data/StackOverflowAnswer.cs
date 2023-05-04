using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompany.Data
{
    public class StackOverflowAnswer
    {
        public int Id { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [Required]
        public string  UserName { get; set; }

        [Required]
        public string Answer { get; set; }
    }
}
