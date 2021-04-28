using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Promotion
{
    public class SearchListPromotionParameter : BaseParameter
    {
        public string PromotionCode { get; set; }
        public string PromotionName { get; set; }
        public DateTime? ExpirationDateFrom { get; set; }
        public DateTime? ExpirationDateTo { get; set; }
        public int? QuantityOrder { get; set; }
        public int? QuantityQuote { get; set; }
        public int? QuantityContract { get; set; }
    }
}
