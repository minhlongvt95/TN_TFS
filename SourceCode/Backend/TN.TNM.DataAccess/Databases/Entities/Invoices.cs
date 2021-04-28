using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Invoices
    {
        public Guid InvoiceId { get; set; }
        public Guid ObjectId { get; set; }
        public string ObjectType { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal? Tax { get; set; }
        public decimal? Amount { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public Guid? TenantId { get; set; }
    }
}
