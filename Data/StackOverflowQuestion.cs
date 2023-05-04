﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompany.Data
{
    public class StackOverflowQuestion
    {
        public int Id { get; set; }
        
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Question { get; set; }
    }
}
