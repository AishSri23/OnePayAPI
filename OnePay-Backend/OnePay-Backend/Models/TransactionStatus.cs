using System;
using System.Collections.Generic;

namespace OnePay_Backend.Models
{
    public partial class TransactionStatus
    {
        public TransactionStatus()
        {
            OnePayTransaction = new HashSet<OnePayTransaction>();
        }

        public int TransactionStatusId { get; set; }
        public string TransactionType { get; set; }

        public ICollection<OnePayTransaction> OnePayTransaction { get; set; }
    }
}
