using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Promotion;
using TN.TNM.DataAccess.Models.Promotion;

namespace TN.TNM.BusinessLogic.Messages.Requests.Promotion
{
    public class CreatePromotionRequest : BaseRequest<CreatePromotionParameter>
    {
        public PromotionEntityModel Promotion { get; set; }
        public List<PromotionMappingEntityModel> ListPromotionMapping { get; set; }

        public override CreatePromotionParameter ToParameter()
        {
            return new CreatePromotionParameter()
            {
                UserId = UserId,
                Promotion = Promotion,
                ListPromotionMapping = ListPromotionMapping
            };
        }
    }
}
