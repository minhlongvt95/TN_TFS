using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class GetCustomerOrderBySellerParameter : BaseParameter
    {
        public Guid Seller { get; set; }
        public DateTime? OrderDateStart { get; set; }
        public DateTime? OrderDateEnd { get; set; }
    }
}
