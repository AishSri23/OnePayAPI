using System;
using System.Collections.Generic;

namespace OnePay_Backend.Models
{
    public partial class BusinessProduct
    {
        public BusinessProduct()
        {
            OnePayTransaction = new HashSet<OnePayTransaction>();
        }

        public string PaccountId { get; set; }
        public string UserId { get; set; }
        public string BproductName { get; set; }
        public int BproductTypeId { get; set; }
        public decimal? Balance { get; set; }
        public int? PaymentCycleTypeId { get; set; }
        public decimal? MinimumPayment { get; set; }
        public decimal? MonthlyCyclePayment { get; set; }
        public decimal? TotalPayment { get; set; }
        public bool? CanBeModified { get; set; }

        public BproductType BproductType { get; set; }
        public PaymentCycleType PaymentCycleType { get; set; }
        public User User { get; set; }
        public ICollection<OnePayTransaction> OnePayTransaction { get; set; }
    }
}
