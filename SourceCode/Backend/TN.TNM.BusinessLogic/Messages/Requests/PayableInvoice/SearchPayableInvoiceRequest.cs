using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.PayableInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.PayableInvoice
{
    public class SearchPayableInvoiceRequest : BaseRequest<SearchPayableInvoiceParameter>
    {
        public string PayableInvoiceCode { get; set; }
        public List<Guid> PayableReasonIdList { get; set; }
        public List<Guid> CreatedByIdList { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<Guid> SttList { get; set; }
        public List<Guid> ObjectIdList { get; set; }
        public override SearchPayableInvoiceParameter ToParameter()
        {
            return new SearchPayableInvoiceParameter() {
                UserId = UserId,
                CreatedByIdList = CreatedByIdList,
                FromDate = FromDate,
                ToDate = ToDate,
                PayableInvoiceCode = PayableInvoiceCode,
                PayableReasonIdList = PayableReasonIdList,
                SttList = SttList,
                ObjectIdList = ObjectIdList
            };
        }
    }
}
