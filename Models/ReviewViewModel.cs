using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompany.Models
{
    public class ReviewViewModel
    {
        public string Name { get; set; }
        
        [Required]
        public string Review { get; set; }
    }
}
