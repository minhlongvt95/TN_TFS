using TN.TNM.BusinessLogic.Models.PayableInvoice;

namespace TN.TNM.BusinessLogic.Messages.Responses.PayableInvoice
{
    public class GetPayableInvoiceByIdResponse : BaseResponse
    {
        public PayableInvoiceModel PayableInvoice { get; set; }
        //public string PayerName { get; set; }
        //public string CreatedByName { get; set; }
    }
}
