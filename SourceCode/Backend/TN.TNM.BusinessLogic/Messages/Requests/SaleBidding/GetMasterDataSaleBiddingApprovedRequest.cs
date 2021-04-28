using System;
using TN.TNM.DataAccess.Messages.Parameters.SaleBidding;

namespace TN.TNM.BusinessLogic.Messages.Requests.SaleBidding
{
    public class GetMasterDataSaleBiddingApprovedRequest : BaseRequest<GetMasterDataSaleBiddingApprovedParameter>
    {
        public override GetMasterDataSaleBiddingApprovedParameter ToParameter()
        {
            return new GetMasterDataSaleBiddingApprovedParameter()
            {
                UserId = UserId,
            };
        }
    }
}
