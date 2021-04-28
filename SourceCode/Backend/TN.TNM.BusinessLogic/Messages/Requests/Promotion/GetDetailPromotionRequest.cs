using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Promotion;

namespace TN.TNM.BusinessLogic.Messages.Requests.Promotion
{
    public class GetDetailPromotionRequest : BaseRequest<GetDetailPromotionParameter>
    {
        public Guid PromotionId { get; set; }

        public override GetDetailPromotionParameter ToParameter()
        {
            return new GetDetailPromotionParameter()
            {
                UserId = UserId,
                PromotionId = PromotionId
            };
        }
    }
}
