using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePay_Backend.DTO
{
    public class BusinessProductDTO
    {
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
    }
}
