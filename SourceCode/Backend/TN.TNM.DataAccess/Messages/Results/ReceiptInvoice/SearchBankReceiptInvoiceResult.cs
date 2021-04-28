using System.Collections.Generic;
using TN.TNM.DataAccess.Models.ReceiptInvoice;

namespace TN.TNM.DataAccess.Messages.Results.ReceiptInvoice
{
    public class SearchBankReceiptInvoiceResult: BaseResult
    {
        public List<BankReceiptInvoiceEntityModel> BankReceiptInvoiceList { get; set; }
    }
}
