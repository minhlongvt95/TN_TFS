using System;

namespace TN.TNM.DataAccess.Models.PayableInvoice
{
    public class PayableInvoiceMappingEntityModel
    {
        public Guid PayableInvoiceMappingId { get; set; }
        public Guid PayableInvoiceId { get; set; }
        public Guid ObjectId { get; set; }
        public short? ReferenceType { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
