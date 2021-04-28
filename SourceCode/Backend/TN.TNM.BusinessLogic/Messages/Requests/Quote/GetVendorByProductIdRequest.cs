using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class GetVendorByProductIdRequest : BaseRequest<GetVendorByProductIdParameter>
    {
        public Guid ProductId { get; set; }
        public DateTime? OrderDate { get; set; }
        public Guid? CustomerGroupId { get; set; }

        public override GetVendorByProductIdParameter ToParameter()
        {
            return new GetVendorByProductIdParameter()
            {
                ProductId = ProductId,
                OrderDate = OrderDate,
                CustomerGroupId = CustomerGroupId,
                UserId = UserId
            };
        }
    }
}
