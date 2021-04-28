using System;
using TN.TNM.DataAccess.Messages.Parameters.SaleBidding;

namespace TN.TNM.BusinessLogic.Messages.Requests.SaleBidding
{
    public class GetVendorMappingRequest : BaseRequest<GetVendorMappingParameter>
    {
        public Guid ProductId { get; set; }
        public override GetVendorMappingParameter ToParameter()
        {
            return new GetVendorMappingParameter()
            {
                ProductId = ProductId,
                UserId = UserId
            };
        }
    }
}
