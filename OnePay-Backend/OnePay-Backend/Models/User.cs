using System;
using System.Collections.Generic;

namespace OnePay_Backend.Models
{
    public partial class User
    {
        public User()
        {
            BusinessProduct = new HashSet<BusinessProduct>();
            Message = new HashSet<Message>();
            OnePayTransaction = new HashSet<OnePayTransaction>();
            UserAccount = new HashSet<UserAccount>();
        }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserPhoneNumber { get; set; }
        public string UserEmailAddress { get; set; }
        public int UserStatusId { get; set; }
        public bool IsRegisteredForOnePay { get; set; }
        public string Password { get; set; }

        public UserStatus UserStatus { get; set; }
        public ICollection<BusinessProduct> BusinessProduct { get; set; }
        public ICollection<Message> Message { get; set; }
        public ICollection<OnePayTransaction> OnePayTransaction { get; set; }
        public ICollection<UserAccount> UserAccount { get; set; }
    }
}
