using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Messages.Responses.ReceiptInvoice
{
    public class SearchReceiptInvoiceResponse : BaseResponse
    {
        public List<ReceiptInvoiceModel> lstReceiptInvoiceEntity { get; set; }
    }
}
