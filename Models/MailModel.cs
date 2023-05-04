using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompany.Models
{
    public class MailModel
    {

        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        public string YourMail { get; set; }

        public List<string> ToMail { get; set; }

        public string ToMails { get; set; }

        public string Subject { get; set; }

        public string Phone { get; set; }

        public string Body { get; set; }

    }
}
