using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Promotion
{
    public class CheckPromotionByCustomerResponse : BaseResponse
    {
        public bool IsPromotionCustomer { get; set; }
    }
}
