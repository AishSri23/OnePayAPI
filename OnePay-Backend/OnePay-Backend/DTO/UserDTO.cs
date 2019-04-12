using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePay_Backend.DTO
{
    public class UserDTO
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserPhoneNumber { get; set; }
        public string UserEmailAddress { get; set; }
        public string Password { get; set; }
        public bool IsRegisteredForOnePay { get; set; }
    }
}
