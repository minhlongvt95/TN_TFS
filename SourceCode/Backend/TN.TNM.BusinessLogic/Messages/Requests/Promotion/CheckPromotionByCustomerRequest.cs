using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Promotion;

namespace TN.TNM.BusinessLogic.Messages.Requests.Promotion
{
    public class CheckPromotionByCustomerRequest : BaseRequest<CheckPromotionByCustomerParameter>
    {
        public Guid CustomerId { get; set; }

        public override CheckPromotionByCustomerParameter ToParameter()
        {
            return new CheckPromotionByCustomerParameter()
            {
                UserId = UserId,
                CustomerId = CustomerId
            };
        }
    }
}
