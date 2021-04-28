using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Promotion
{
    public class CheckPromotionByAmountResponse : BaseResponse
    {
        public bool IsPromotionAmount { get; set; }
    }
}
