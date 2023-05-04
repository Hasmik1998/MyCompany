using MyCompany.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompany.Models
{
    public class ChatRoomsModel
    {
        public int ChatRoomId { get; set; }

        public string ToUserUserName { get; set; }

        public string Image { get; set; }

        public DateTime ChatRoomData { get; set; }
    }
}
