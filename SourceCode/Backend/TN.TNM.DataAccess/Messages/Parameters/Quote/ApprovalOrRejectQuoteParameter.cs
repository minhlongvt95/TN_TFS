using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class ApprovalOrRejectQuoteParameter : BaseParameter
    {
        public List<Guid> ListQuoteId { get; set; }
        public bool IsApproval { get; set; }
        public string Description { get; set; }
        public string RejectReason { get; set; }
    }
}
