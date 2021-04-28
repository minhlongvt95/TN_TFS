using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Promotion;

namespace TN.TNM.BusinessLogic.Messages.Requests.Promotion
{
    public class GetMasterDataListPromotionRequest : BaseRequest<GetMasterDataListPromotionParameter>
    {
        public override GetMasterDataListPromotionParameter ToParameter()
        {
            return new GetMasterDataListPromotionParameter()
            {
                UserId = UserId
            };
        }
    }
}
