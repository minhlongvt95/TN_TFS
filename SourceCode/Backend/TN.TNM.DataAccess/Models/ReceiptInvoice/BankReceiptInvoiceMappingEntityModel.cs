using System;

namespace TN.TNM.DataAccess.Models.ReceiptInvoice
{
    public class BankReceiptInvoiceMappingEntityModel
    {
        public Guid BankReceiptInvoiceMappingId { get; set; }
        public Guid BankReceiptInvoiceId { get; set; }
        public Guid? ObjectId { get; set; }
        public short? ReferenceType { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
