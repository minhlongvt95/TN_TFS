using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Promotion;

namespace TN.TNM.BusinessLogic.Messages.Requests.Promotion
{
    public class CheckPromotionByProductRequest : BaseRequest<CheckPromotionByProductParameter>
    {
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }

        public override CheckPromotionByProductParameter ToParameter()
        {
            return new CheckPromotionByProductParameter()
            {
                UserId = UserId,
                ProductId = ProductId,
                Quantity = Quantity
            };
        }
    }
}
