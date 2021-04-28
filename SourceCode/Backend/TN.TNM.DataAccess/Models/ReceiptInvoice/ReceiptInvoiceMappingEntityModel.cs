using System;

namespace TN.TNM.DataAccess.Models.ReceiptInvoice
{
    public class ReceiptInvoiceMappingEntityModel
    {
        public Guid ReceiptInvoiceMappingId { get; set; }
        public Guid ReceiptInvoiceId { get; set; }
        public Guid ObjectId { get; set; }
        public short? ReferenceType { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
