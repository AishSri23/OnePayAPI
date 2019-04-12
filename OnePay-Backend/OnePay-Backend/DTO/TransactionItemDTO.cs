using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePay_Backend.DTO
{
    public class TransactionItemDTO
    {
        public string UserId { get; set; }
        public string AccountId { get; set; }
        public string PAccountId { get; set; }
        public decimal TransactionAmount { get; set; }
        public int TransactionType { get; set; }
        public int TransactionProviderType { get; set; }
    }
}
