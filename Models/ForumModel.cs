using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompany.Models
{
    public class ForumModel
    {
        public List<MyCompany.Data.StackOverflowQuestion> Questions { get; set; }

        public string SearchText { get; set; }
    }
}
