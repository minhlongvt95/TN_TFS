using System;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class GetCustomerOrderBySellerRequest : BaseRequest<GetCustomerOrderBySellerParameter>
    {
        public Guid Seller { get; set; }
        public DateTime? OrderDateStart { get; set; }
        public DateTime? OrderDateEnd { get; set; }
        public override GetCustomerOrderBySellerParameter ToParameter() => new GetCustomerOrderBySellerParameter()
        {
            Seller = Seller,
            OrderDateStart = OrderDateStart,
            OrderDateEnd = OrderDateEnd
        };
    }
}
