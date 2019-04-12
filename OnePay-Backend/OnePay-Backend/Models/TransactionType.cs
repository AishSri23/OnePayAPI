using System;
using System.Collections.Generic;

namespace OnePay_Backend.Models
{
    public partial class TransactionType
    {
        public TransactionType()
        {
            OnePayTransaction = new HashSet<OnePayTransaction>();
        }

        public int TransactionTypeId { get; set; }
        public string TransactionsType { get; set; }

        public ICollection<OnePayTransaction> OnePayTransaction { get; set; }
    }
}
