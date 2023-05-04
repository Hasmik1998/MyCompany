using MyCompany.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompany.Models
{
    public class MessagesModel
    {
        public List<Message> Messages { get; set; }

        public string UserImage { get; set; }
    }
}
