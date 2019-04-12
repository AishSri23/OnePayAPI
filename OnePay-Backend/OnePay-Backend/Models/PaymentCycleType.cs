using System;
using System.Collections.Generic;

namespace OnePay_Backend.Models
{
    public partial class PaymentCycleType
    {
        public PaymentCycleType()
        {
            BusinessProduct = new HashSet<BusinessProduct>();
        }

        public int PaymentCycleId { get; set; }
        public string PaymentCycleType1 { get; set; }

        public ICollection<BusinessProduct> BusinessProduct { get; set; }
    }
}
