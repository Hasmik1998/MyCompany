using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompany.Models
{
    public class Users : IdentityUser
    {
        public string CategotyName { get; set; }
    }
}
