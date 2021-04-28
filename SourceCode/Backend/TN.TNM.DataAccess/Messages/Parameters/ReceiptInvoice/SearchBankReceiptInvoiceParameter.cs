using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice
{
    public class SearchBankReceiptInvoiceParameter : BaseParameter
    {
        public string ReceiptInvoiceCode { get; set; }
        public List<Guid> ReceiptReasonIdList { get; set; }
        public List<Guid> CreatedByIdList { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<Guid> SttList { get; set; }
        public List<Guid> ObjectIdList { get; set; }
    }
}
