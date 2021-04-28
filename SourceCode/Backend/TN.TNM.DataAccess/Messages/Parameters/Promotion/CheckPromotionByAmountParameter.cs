using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Promotion
{
    public class CheckPromotionByAmountParameter : BaseParameter
    {
        public decimal Amount { get; set; }
    }
}
