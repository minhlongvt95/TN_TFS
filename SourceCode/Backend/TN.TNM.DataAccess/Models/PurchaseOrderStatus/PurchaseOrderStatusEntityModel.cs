using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.PurchaseOrderStatus
{
    public class PurchaseOrderStatusEntityModel
    {
        public Guid PurchaseOrderStatusId { get; set; }
        public string PurchaseOrderStatusCode { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
