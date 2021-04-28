using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.PayableInvoice;

namespace TN.TNM.BusinessLogic.Messages.Responses.PayableInvoice
{
    public class SearchPayableInvoiceResponse : BaseResponse
    {
        public List<PayableInvoiceModel> PayableInvList { get; set; }
    }
}
