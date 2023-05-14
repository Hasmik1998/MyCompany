using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompany.Data
{
    public class PasswordSecurity
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int MinimumLength { get; set; }
        [Required]
        public int LowercaseNum { get; set; }
        [Required]
        public int UpercaseNum { get; set; }
        [Required]
        public int NumberCharacters { get; set; }
        [Required]
        public int SpecialCharacters { get; set; }
    }
}
