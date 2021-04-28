using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class InventoryDeliveryVoucherSerialMapping
    {
        public Guid InventoryDeliveryVoucherSerialMappingId { get; set; }
        public Guid InventoryDeliveryVoucherMappingId { get; set; }
        public Guid SerialId { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
    }
}
