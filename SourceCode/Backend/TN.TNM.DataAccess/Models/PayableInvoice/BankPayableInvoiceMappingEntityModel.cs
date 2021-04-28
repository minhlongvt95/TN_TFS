using System;

namespace TN.TNM.DataAccess.Models.PayableInvoice
{
    public class BankPayableInvoiceMappingEntityModel
    {
        public Guid BankPayableInvoiceMappingId { get; set; }
        public Guid BankPayableInvoiceId { get; set; }
        public Guid? ObjectId { get; set; }
        public short? ReferenceType { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
