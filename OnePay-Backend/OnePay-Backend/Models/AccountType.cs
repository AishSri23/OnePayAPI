using System;
using System.Collections.Generic;

namespace OnePay_Backend.Models
{
    public partial class AccountType
    {
        public AccountType()
        {
            BproductType = new HashSet<BproductType>();
            UserAccount = new HashSet<UserAccount>();
        }

        public int AccountTypeId { get; set; }
        public string AccountType1 { get; set; }

        public ICollection<BproductType> BproductType { get; set; }
        public ICollection<UserAccount> UserAccount { get; set; }
    }
}
