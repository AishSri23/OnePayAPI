using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePay_Backend.DTO
{
    public class UserAccountDTO
    {
        public string AccountId { get; set; }
        public int? AccountTypeId { get; set; }
        public string AccountName { get; set; }
        public decimal? AccountBalance { get; set; }
        public decimal? InterestRate { get; set; }
        public byte[] Timestamp { get; set; }
        public string UserId { get; set; }
    }
}
