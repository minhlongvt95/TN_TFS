using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.SaleBidding;

namespace TN.TNM.BusinessLogic.Messages.Requests.SaleBidding
{
    public class GetVendorByProductIdRequest:BaseRequest<GetVendorByProductIdParameter>
    {
        public Guid ProductId { get; set; }

        public override GetVendorByProductIdParameter ToParameter()
        {
            return new GetVendorByProductIdParameter()
            {
                ProductId = ProductId,
                UserId = UserId
            };
        }
    }
}
