using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class SearchRevenueProductParameter: BaseParameter
    {
        public string ProductCode { get; set; }
        public List<Guid?> ProductCategory { get; set; }
        public DateTime? OrderFromDate { get; set; }
        public DateTime? OrderToDate { get; set; }
        public decimal? SaleRevenueFrom { get; set; }
        public decimal? SaleRevenueTo { get; set; }
        public decimal? ProductInOrderCountFrom { get; set; }
        public decimal? ProductInOrderCountTo { get; set; }
        public decimal? OrderCountFrom { get; set; }
        public decimal? OrderCountTo { get; set; }
        public decimal? ProductRefundCountFrom { get; set; }
        public decimal? ProductRefundCountTo { get; set; }
        public List<Guid?> Warehouse { get; set; }
        public Guid? Seller { get; set; }
        public List<Guid?> CustomerGroup { get; set; }
        public List<short?> CustomerType { get; set; }
        public List<Guid?> Customer { get; set; }
    }
}
