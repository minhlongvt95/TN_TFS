using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice
{
    public class SearchReceiptInvoiceParameter : BaseParameter
    {
        public string ReceiptInvoiceCode { get; set; }
        public List<Guid?> ReceiptInvoiceReason { get; set; }
        public List<Guid> CreateById { get; set; }
        public DateTime? CreateDateFrom { get; set; }
        public DateTime? CreateDateTo { get; set; }
        public List<Guid?> Status { get; set; }
        public List<Guid?> ObjectReceipt { get; set; }
    }
}
