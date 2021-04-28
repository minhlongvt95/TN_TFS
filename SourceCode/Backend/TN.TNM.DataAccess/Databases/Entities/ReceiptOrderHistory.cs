using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ReceiptOrderHistory
    {
        public Guid ReceiptOrderHistoryId { get; set; }
        public Guid ObjectId { get; set; }
        public string ObjectType { get; set; }
        public Guid OrderId { get; set; }
        public decimal AmountCollected { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? TenantId { get; set; }
    }
}
