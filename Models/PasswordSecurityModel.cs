using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompany.Models
{
    public class PasswordSecurityModel
    {
        public int MinimumLength { get; set; }
        public int LowercaseNum { get; set; }
        public int UppercaseNum { get; set; }
        public int NumberCharacters { get; set; }
        public int SpecialCharacters { get; set; }
    }
}
