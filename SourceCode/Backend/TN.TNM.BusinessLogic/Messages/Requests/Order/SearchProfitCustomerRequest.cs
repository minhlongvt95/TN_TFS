using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class SearchProfitCustomerRequest : BaseRequest<SearchProfitCustomerParameter>
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

        public override SearchProfitCustomerParameter ToParameter()
        {
            return new SearchProfitCustomerParameter()
            {
                ListCustomer = ListCustomer ?? new List<Guid?>(),
                ListCustomerType = ListCustomerType ?? new List<short?>(),
                ListCustomerGroup = ListCustomerGroup ?? new List<Guid?>(),
                OrderFromDate = OrderFromDate,
                OrderToDate = OrderToDate,
                SaleRevenueFrom = SaleRevenueFrom,
                SaleRevenueTo = SaleRevenueTo,
                OrderCode = OrderCode,
                ListProductCategory = ListProductCategory ?? new List<Guid?>(),
                ListContract = ListContract ?? new List<Guid?>(),
                ProductCode = ProductCode,
                QuoteCode = QuoteCode,
                UserId = UserId
            };
        }
    }
}
