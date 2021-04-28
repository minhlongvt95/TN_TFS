using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Promotion;

namespace TN.TNM.BusinessLogic.Messages.Requests.Promotion
{
    public class GetApplyPromotionRequest : BaseRequest<GetApplyPromotionParameter>
    {
        public int ConditionsType { get; set; }
        public Guid? CustomerId { get; set; }
        public decimal Amount { get; set; }
        public Guid? ProductId { get; set; }
        public decimal Quantity { get; set; }

        public override GetApplyPromotionParameter ToParameter()
        {
            return new GetApplyPromotionParameter()
            {
                UserId = UserId,
                CustomerId = CustomerId,
                ConditionsType = ConditionsType,
                Amount = Amount,
                ProductId = ProductId,
                Quantity = Quantity
            };
        }
    }
}
