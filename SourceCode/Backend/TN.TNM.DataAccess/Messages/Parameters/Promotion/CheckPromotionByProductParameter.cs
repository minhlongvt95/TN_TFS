using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Promotion
{
    public class CheckPromotionByProductParameter : BaseParameter
    {
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
    }
}
