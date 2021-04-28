using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class CheckReceiptOrderHistoryParameter : BaseParameter
    {
        public Guid OrderId { get; set; }
        public decimal MoneyOrder { get; set; }
    }
}
