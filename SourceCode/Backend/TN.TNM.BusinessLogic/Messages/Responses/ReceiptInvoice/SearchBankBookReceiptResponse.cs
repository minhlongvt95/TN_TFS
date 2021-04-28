using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Messages.Responses.ReceiptInvoice
{
    public class SearchBankBookReceiptResponse: BaseResponse
    {
        public List<BankReceiptInvoiceModel> BankReceiptInvoiceList { get; set; }
    }
}
