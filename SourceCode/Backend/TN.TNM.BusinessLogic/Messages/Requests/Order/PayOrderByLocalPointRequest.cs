using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class PayOrderByLocalPointRequest : BaseRequest<PayOrderByLocalPointParameter>
    {
        public List<Guid> ListOrderId { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public bool DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal Point { get; set; }
        public decimal PayPoint { get; set; }

        public override PayOrderByLocalPointParameter ToParameter()
        {
            return new PayOrderByLocalPointParameter()
            {
                UserId = UserId,
                ListOrderId = ListOrderId,
                CustomerId = CustomerId,
                CustomerName = CustomerName,
                CustomerPhone = CustomerPhone,
                DiscountValue = DiscountValue,
                DiscountType = DiscountType,
                PayPoint = PayPoint,
                Point = Point
            };
        }
    }
}
