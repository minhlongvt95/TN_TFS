using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.PayableInvoice
{
    public class SearchBankPayableInvoiceParameter : BaseParameter
    {
        public string PayableInvoiceCode { get; set; }
        public List<Guid> PayableReasonIdList { get; set; }
        public List<Guid> CreatedByIdList { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<Guid> SttList { get; set; }
        public List<Guid> ObjectIdList { get; set; }

    }
}
