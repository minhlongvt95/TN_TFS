using System.Collections.Generic;
using TN.TNM.DataAccess.Models.PayableInvoice;

namespace TN.TNM.DataAccess.Messages.Results.PayableInvoice
{
    public class SearchPayableInvoiceResult : BaseResult
    {
        public List<PayableInvoiceEntityModel> PayableInvList { get; set; }
    }
}
