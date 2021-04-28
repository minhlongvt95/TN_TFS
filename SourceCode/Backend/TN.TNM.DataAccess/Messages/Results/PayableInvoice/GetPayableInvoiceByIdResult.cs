using TN.TNM.DataAccess.Models.PayableInvoice;

namespace TN.TNM.DataAccess.Messages.Results.PayableInvoice
{
    public class GetPayableInvoiceByIdResult : BaseResult
    {
        public PayableInvoiceEntityModel PayableInvoice { get; set; }
        //public string PayerName { get; set; }
        //public string CreatedByName { get; set; }
    }
}
