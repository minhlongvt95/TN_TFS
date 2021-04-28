using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.PayableInvoice;

namespace TN.TNM.BusinessLogic.Messages.Responses.PayableInvoice
{
    public class SearchBankBookPayableInvoiceResponse : BaseResponse
    {
        public List<BankPayableInvoiceModel> BankPayableInvoiceList { get; set; }
    }
}
