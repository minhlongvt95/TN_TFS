using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Promotion;

namespace TN.TNM.BusinessLogic.Messages.Requests.Promotion
{
    public class GetMasterDataCreatePromotionRequest : BaseRequest<GetMasterDataCreatePromotionParameter>
    {
        public override GetMasterDataCreatePromotionParameter ToParameter()
        {
            return new GetMasterDataCreatePromotionParameter()
            {
                UserId = UserId
            };
        }
    }
}
