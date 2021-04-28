using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Quote;

namespace TN.TNM.BusinessLogic.Messages.Requests.Quote
{
    public class ApprovalOrRejectQuoteRequest : BaseRequest<ApprovalOrRejectQuoteParameter>
    {
        public List<Guid> ListQuoteId { get; set; }
        public bool IsApproval { get; set; }
        public string Description { get; set; }
        public string RejectReason { get; set; }
        public override ApprovalOrRejectQuoteParameter ToParameter()
        {
            return new ApprovalOrRejectQuoteParameter
            {
                ListQuoteId = ListQuoteId,
                IsApproval = IsApproval,
                Description = Description,
                RejectReason = RejectReason,
                UserId = UserId
            };
        }
    }
}
