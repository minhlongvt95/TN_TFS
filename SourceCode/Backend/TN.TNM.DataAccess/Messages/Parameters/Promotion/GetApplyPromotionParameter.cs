using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Promotion
{
    public class GetApplyPromotionParameter : BaseParameter
    {
        public int ConditionsType { get; set; }
        public Guid? CustomerId { get; set; }
        public decimal Amount { get; set; }
        public Guid? ProductId { get; set; }
        public decimal Quantity { get; set; }
    }
}
