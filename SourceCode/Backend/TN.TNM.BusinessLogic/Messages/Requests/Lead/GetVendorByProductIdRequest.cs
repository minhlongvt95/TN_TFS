using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
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
                UserId = UserId,
                OrderDate = OrderDate,
                CustomerGroupId = CustomerGroupId
            };
        }
    }
}
