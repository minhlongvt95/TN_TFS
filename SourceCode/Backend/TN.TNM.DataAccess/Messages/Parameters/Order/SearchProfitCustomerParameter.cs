using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class SearchProfitCustomerParameter: BaseParameter
    {
        public List<Guid?> ListCustomer { get; set; }
        public List<short?> ListCustomerType { get; set; }
        public List<Guid?> ListCustomerGroup { get; set; }
        public DateTime? OrderFromDate { get; set; }
        public DateTime? OrderToDate { get; set; }
        public decimal? SaleRevenueFrom { get; set; }
        public decimal? SaleRevenueTo { get; set; }
        public string OrderCode { get; set; }
        public List<Guid?> ListProductCategory { get; set; }
        public List<Guid?> ListContract { get; set; }
        public string ProductCode { get; set; }
        public string QuoteCode { get; set; }
    }
}
