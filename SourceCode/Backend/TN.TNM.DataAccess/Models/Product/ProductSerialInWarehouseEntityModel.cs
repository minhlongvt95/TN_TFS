using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Product
{
    public class ProductSerialInWarehouseEntityModel
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
