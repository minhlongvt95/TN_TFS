using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.PayableInvoice
{
    public class SearchCashBookPayableInvoiceParameter: BaseParameter
    {
        //public string PayableInvoiceCode { get; set; }
        //public List<Guid?> PayableReasonIdList { get; set; }
        public List<Guid> CreatedByIdList { get; set; }
        public DateTime? FromPaidDate { get; set; }
        public DateTime? ToPaidDate { get; set; }
        //public List<Guid?> SttList { get; set; }
        //public List<Guid?> ObjectIdList { get; set; }
        public List<Guid?> OrganizationList { get; set; }
    }
}
