using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Promotion;

namespace TN.TNM.BusinessLogic.Messages.Requests.Promotion
{
    public class CheckPromotionByAmountRequest : BaseRequest<CheckPromotionByAmountParameter>
    {
        public decimal Amount { get; set; }

        public override CheckPromotionByAmountParameter ToParameter()
        {
            return new CheckPromotionByAmountParameter()
            {
                UserId = UserId,
                Amount = Amount
            };
        }
    }
}
