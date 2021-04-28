using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Promotion;

namespace TN.TNM.DataAccess.Messages.Results.Promotion
{
    public class GetApplyPromotionResult : BaseResult
    {
        public List<PromotionApplyModel> ListPromotionApply { get; set; }
    }
}
