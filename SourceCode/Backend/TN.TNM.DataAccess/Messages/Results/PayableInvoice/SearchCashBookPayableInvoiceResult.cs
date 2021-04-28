using System.Collections.Generic;
using TN.TNM.DataAccess.Models.PayableInvoice;

namespace TN.TNM.DataAccess.Messages.Results.PayableInvoice
{
    public class SearchCashBookPayableInvoiceResult : BaseResult
    {
        public List<PayableInvoiceEntityModel> PayableInvList { get; set; }
    }
}
