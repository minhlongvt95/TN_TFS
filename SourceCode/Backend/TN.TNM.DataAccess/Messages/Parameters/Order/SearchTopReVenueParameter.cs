using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class SearchTopReVenueParameter: BaseParameter
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
    }
}
