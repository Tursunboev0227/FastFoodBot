using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFoodBot.Models
{
    public class UsersModel
    {
        public long ChatId { get; set; }
        public short UserStatus { get; set; } = 0;
        public string PhoneNumber { get; set; }
    }
}
