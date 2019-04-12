using System;
using System.Collections.Generic;

namespace OnePay_Backend.Models
{
    public partial class UserStatus
    {
        public UserStatus()
        {
            User = new HashSet<User>();
        }

        public int UserStatusId { get; set; }
        public string UserStatusType { get; set; }

        public ICollection<User> User { get; set; }
    }
}
