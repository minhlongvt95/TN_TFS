using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Quote
{
    public class QuoteDetailToSendEmailModel
    {
        public string ProductName { get; set; }
        public string ProductUnitName { get; set; }
        public string UnitPrice { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Vat { get; set; }
        public decimal? DiscountValue { get; set; }
    }
}
