using System;
using TN.TNM.DataAccess.Messages.Parameters.SaleBidding;

namespace TN.TNM.BusinessLogic.Messages.Requests.SaleBidding
{
    public class GetMasterDataSaleBiddingDetailRequest : BaseRequest<GetMasterDataSaleBiddingDetailParameter>
    {
        public Guid SaleBiddingId { get; set; }

        public override GetMasterDataSaleBiddingDetailParameter ToParameter()
        {
            return new GetMasterDataSaleBiddingDetailParameter()
            {
                SaleBiddingId = SaleBiddingId,
                UserId = UserId
            };
        }
    }
}
