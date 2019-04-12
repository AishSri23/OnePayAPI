using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePay_Backend.DTO
{
    public class TransactionDTO
    {
        public string TransactionId { get; set; }
        public string TransactionComment { get; set; }
        public string AccountId { get; set; }
        public string PaccountId { get; set; }
        public string UserId { get; set; }
        public byte[] Timestamp { get; set; }
        public int TransactionTypeId { get; set; }
        public int TransactionStatusId { get; set; }
        public decimal TransactionAmount { get; set; }
        public int TransactionProvideTypeId { get; set; }
    }
}
