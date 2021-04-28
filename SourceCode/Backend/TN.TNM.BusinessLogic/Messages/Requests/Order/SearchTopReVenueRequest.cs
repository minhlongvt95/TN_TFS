using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class SearchTopReVenueRequest : BaseRequest<SearchTopReVenueParameter>
    {
        public List<Guid?> ListSellerId { get; set; }
        public Guid? OrganizationId { get; set; }
        public DateTime? OrderFromDate { get; set; }
        public DateTime? OrderToDate { get; set; }
        public decimal? AmountFrom { get; set; }
        public decimal? AmountTo { get; set; }
        public decimal? TotalProductInOrderFrom { get; set; }
        public decimal? TotalProductInOrderTo { get; set; }
        public decimal? TotalOrderFrom { get; set; }
        public decimal? TotalOrderTo { get; set; }
        public List<short?> ListCustomerType { get; set; }
        public List<Guid?> ListCustomer { get; set; }
        public List<Guid?> ListCustomerGroup { get; set; }
        public List<Guid?> ListProductGroup { get; set; }
        public string ProductCode { get; set; }

        public override SearchTopReVenueParameter ToParameter()
        {
            return new SearchTopReVenueParameter()
            {
                ListSellerId = ListSellerId ?? new List<Guid?>(),
                OrganizationId = OrganizationId,
                OrderFromDate = OrderFromDate,
                OrderToDate = OrderToDate,
                AmountFrom = AmountFrom,
                AmountTo = AmountTo,
                TotalProductInOrderFrom = TotalProductInOrderFrom,
                TotalProductInOrderTo = TotalProductInOrderTo,
                TotalOrderFrom = TotalOrderFrom,
                TotalOrderTo = TotalOrderTo,
                ListCustomerType = ListCustomerType ?? new List<short?>(),
                ListCustomer = ListCustomer ?? new List<Guid?>(),
                ListCustomerGroup = ListCustomerGroup ?? new List<Guid?>(),
                ListProductGroup = ListProductGroup ?? new List<Guid?>(),
                ProductCode = ProductCode,

                UserId = UserId
            };
        }
    }
}
