using System;
using System.Collections.Generic;

namespace OnePay_Backend.Models
{
    public partial class OnePayTransaction
    {
        public string TransactionId { get; set; }
        public string TransactionComment { get; set; }
        public string AccountId { get; set; }
        public string PaccountId { get; set; }
        public string UserId { get; set; }
        public string Timestamp { get; set; }
        public int TransactionTypeId { get; set; }
        public int TransactionStatusId { get; set; }
        public decimal TransactionAmount { get; set; }
        public int TransactionProvideTypeId { get; set; }

        public UserAccount Account { get; set; }
        public BusinessProduct Paccount { get; set; }
        public TransactionProviderType TransactionProvideType { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
        public TransactionType TransactionType { get; set; }
        public User User { get; set; }
    }
}
