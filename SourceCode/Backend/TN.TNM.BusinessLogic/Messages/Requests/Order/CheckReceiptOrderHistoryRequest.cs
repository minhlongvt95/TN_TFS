using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Contact;
using TN.TNM.BusinessLogic.Models.Order;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.Order;

namespace TN.TNM.BusinessLogic.Messages.Requests.Order
{
    public class CheckReceiptOrderHistoryRequest : BaseRequest<CheckReceiptOrderHistoryParameter>
    {
        public Guid OrderId { get; set; }
        public decimal MoneyOrder { get; set; }
        public override CheckReceiptOrderHistoryParameter ToParameter()
        {
            return new CheckReceiptOrderHistoryParameter
            {
                OrderId = OrderId,
                MoneyOrder = MoneyOrder,
                UserId =this.UserId
            };
        }
    }
}
