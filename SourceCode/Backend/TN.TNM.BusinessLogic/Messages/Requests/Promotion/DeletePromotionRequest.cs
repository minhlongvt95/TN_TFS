using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Promotion;

namespace TN.TNM.BusinessLogic.Messages.Requests.Promotion
{
    public class DeletePromotionRequest : BaseRequest<DeletePromotionParameter>
    {
        public Guid PromotionId { get; set; }

        public override DeletePromotionParameter ToParameter()
        {
            return new DeletePromotionParameter()
            {
                UserId = UserId,
                PromotionId = PromotionId
            };
        }
    }
}
