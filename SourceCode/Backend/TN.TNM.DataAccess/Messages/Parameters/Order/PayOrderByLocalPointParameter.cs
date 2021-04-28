using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class PayOrderByLocalPointParameter : BaseParameter
    {
        public List<Guid> ListOrderId { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public bool DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal Point { get; set; }
        public decimal PayPoint { get; set; }
    }
}
