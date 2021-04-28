using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.ReceiptInvoice
{
    public class SearchBankReceiptInvoiceRequest : BaseRequest<SearchBankReceiptInvoiceParameter>
    {

        public string ReceiptInvoiceCode { get; set; }
        public List<Guid> ReceiptReasonIdList { get; set; }
        public List<Guid> CreatedByIdList { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<Guid> SttList { get; set; }
        public List<Guid> ObjectIdList { get; set; }
        public override SearchBankReceiptInvoiceParameter ToParameter()
        {
            return new SearchBankReceiptInvoiceParameter()
            {
                UserId = UserId,
                CreatedByIdList = CreatedByIdList,
                FromDate = FromDate,
                ToDate = ToDate,
                ReceiptInvoiceCode = ReceiptInvoiceCode,
                ReceiptReasonIdList = ReceiptReasonIdList,
                SttList = SttList,
                ObjectIdList = ObjectIdList
            };
        }

    }
}
