using System.Collections.Generic;
using TN.TNM.DataAccess.Models.ReceiptInvoice;

namespace TN.TNM.DataAccess.Messages.Results.ReceiptInvoice
{
    public class SearchReceiptInvoiceResult : BaseResult
    {
        public List<ReceiptInvoiceEntityModel> lstReceiptInvoiceEntity { get; set;}
    }
}
