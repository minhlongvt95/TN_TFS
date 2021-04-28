using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class GetVendorByProductIdRequest : BaseRequest<GetVendorByProductIdParameter>
    {
        public Guid ProductId { get; set; }
        public Guid? CustomerGroupId { get; set; }
        public DateTime OrderDate { get; set; }
        public override GetVendorByProductIdParameter ToParameter()
        {
            return new GetVendorByProductIdParameter()
            {
                UserId = UserId,
                CustomerGroupId = CustomerGroupId,
                OrderDate = OrderDate,
                ProductId = ProductId
            };
        }
    }
}
