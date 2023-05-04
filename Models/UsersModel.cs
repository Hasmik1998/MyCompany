using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompany.Models
{
    public class UsersModel
    {
        public List<MyCompany.Data.RegistrationRequest> Users { get; set; }

        public string SearchText { get; set; }
    }
}
