using System.Collections.Generic;
using TN.TNM.DataAccess.Models.PayableInvoice;

namespace TN.TNM.DataAccess.Messages.Results.PayableInvoice
{
    public class SearchBankBookPayableInvoiceResult : BaseResult
    {
        public List<BankPayableInvoiceEntityModel> BankPayableInvoiceList { get; set; }
    }
}
