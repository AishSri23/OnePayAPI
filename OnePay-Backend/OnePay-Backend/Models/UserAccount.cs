using System;
using System.Collections.Generic;

namespace OnePay_Backend.Models
{
    public partial class UserAccount
    {
        public UserAccount()
        {
            OnePayTransaction = new HashSet<OnePayTransaction>();
        }

        public string AccountId { get; set; }
        public int? AccountTypeId { get; set; }
        public string AccountName { get; set; }
        public decimal? AccountBalance { get; set; }
        public decimal? InterestRate { get; set; }
        public byte[] Timestamp { get; set; }
        public string UserId { get; set; }

        public AccountType AccountType { get; set; }
        public User User { get; set; }
        public ICollection<OnePayTransaction> OnePayTransaction { get; set; }
    }
}
