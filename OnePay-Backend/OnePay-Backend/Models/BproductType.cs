using System;
using System.Collections.Generic;

namespace OnePay_Backend.Models
{
    public partial class BproductType
    {
        public BproductType()
        {
            BusinessProduct = new HashSet<BusinessProduct>();
        }

        public int BproductTypeId { get; set; }
        public string BproductType1 { get; set; }
        public int AccountTypeId { get; set; }

        public AccountType AccountType { get; set; }
        public ICollection<BusinessProduct> BusinessProduct { get; set; }
    }
}
