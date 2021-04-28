using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.SaleBidding;

namespace TN.TNM.BusinessLogic.Messages.Requests.SaleBidding
{
    public class GetMasterDataSearchSaleBiddingRequest : BaseRequest<GetMasterDataSearchSaleBiddingParamter>
    {
        public override GetMasterDataSearchSaleBiddingParamter ToParameter()
        {
            return new GetMasterDataSearchSaleBiddingParamter
            {
                UserId = UserId
            };
        }
    }
}
