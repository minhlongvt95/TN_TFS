using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Promotion;

namespace TN.TNM.BusinessLogic.Messages.Requests.Promotion
{
    public class GetMasterDataDetailPromotionRequest : BaseRequest<GetMasterDataDetailPromotionParameter>
    {
        public override GetMasterDataDetailPromotionParameter ToParameter()
        {
            return new GetMasterDataDetailPromotionParameter()
            {
                UserId = UserId
            };
        }
    }
}
