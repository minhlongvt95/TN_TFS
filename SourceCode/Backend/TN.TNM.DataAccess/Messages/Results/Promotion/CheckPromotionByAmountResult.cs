using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Promotion
{
    public class CheckPromotionByAmountResult : BaseResult
    {
        public bool IsPromotionAmount { get; set; }
    }
}
