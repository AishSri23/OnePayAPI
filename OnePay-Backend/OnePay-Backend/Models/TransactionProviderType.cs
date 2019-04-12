using System;
using System.Collections.Generic;

namespace OnePay_Backend.Models
{
    public partial class TransactionProviderType
    {
        public TransactionProviderType()
        {
            OnePayTransaction = new HashSet<OnePayTransaction>();
        }

        public int TransactionProviderTypeId { get; set; }
        public string TransactionPtype { get; set; }

        public ICollection<OnePayTransaction> OnePayTransaction { get; set; }
    }
}
