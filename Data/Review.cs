using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompany.Data
{
    public class Review
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Required]
        public string ReviewText { get; set; }
    }
}
