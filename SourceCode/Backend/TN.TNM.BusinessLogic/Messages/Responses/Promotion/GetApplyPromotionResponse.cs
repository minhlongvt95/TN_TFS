using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Promotion;

namespace TN.TNM.BusinessLogic.Messages.Responses.Promotion
{
    public class GetApplyPromotionResponse : BaseResponse
    {
        public List<PromotionApplyModel> ListPromotionApply { get; set; }
    }
}
