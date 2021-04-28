using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class SearchRevenueProductRequest : BaseRequest<SearchRevenueProductParameter>
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


        public override SearchRevenueProductParameter ToParameter()
        {
            return new SearchRevenueProductParameter()
            {
                ProductCode = ProductCode,
                ProductCategory = ProductCategory ?? new List<Guid?>(),
                OrderFromDate = OrderFromDate,
                OrderToDate = OrderToDate,
                SaleRevenueFrom = SaleRevenueFrom,
                SaleRevenueTo = SaleRevenueTo,
                ProductInOrderCountFrom = ProductInOrderCountFrom,
                ProductInOrderCountTo = ProductInOrderCountTo,
                OrderCountFrom = OrderCountFrom,
                OrderCountTo = OrderCountTo,
                ProductRefundCountFrom = ProductRefundCountFrom,
                ProductRefundCountTo = ProductRefundCountTo,
                Warehouse = Warehouse ?? new List<Guid?>(),
                Seller = Seller,
                CustomerGroup = CustomerGroup ?? new List<Guid?>(),
                CustomerType = CustomerType ?? new List<short?>(),
                Customer = Customer ?? new List<Guid?>(),

                UserId = UserId
            };
        }
    }
}
