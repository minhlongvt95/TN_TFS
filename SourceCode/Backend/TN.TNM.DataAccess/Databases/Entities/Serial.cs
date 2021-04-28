using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Serial
    {
        public Guid SerialId { get; set; }
        public string SerialCode { get; set; }
        public Guid ProductId { get; set; }
        public Guid StatusId { get; set; }
        public Guid? WarehouseId { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
    }
}
