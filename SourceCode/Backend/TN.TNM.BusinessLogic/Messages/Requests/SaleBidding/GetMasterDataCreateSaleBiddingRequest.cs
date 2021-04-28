using System;
using TN.TNM.DataAccess.Messages.Parameters.SaleBidding;

namespace TN.TNM.BusinessLogic.Messages.Requests.SaleBidding
{
    public class GetMasterDataCreateSaleBiddingRequest : BaseRequest<GetMasterDataCreateSaleBiddingParameter>
    {
        public Guid LeadId { get; set; }

        public override GetMasterDataCreateSaleBiddingParameter ToParameter()
        {
            return new GetMasterDataCreateSaleBiddingParameter()
            {
                LeadId = LeadId,
                UserId = UserId
            };
        }
    }
}
