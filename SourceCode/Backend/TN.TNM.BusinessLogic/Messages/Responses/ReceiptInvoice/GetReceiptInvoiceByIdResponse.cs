using TN.TNM.BusinessLogic.Models.ReceiptInvoice;

namespace TN.TNM.BusinessLogic.Messages.Responses.ReceiptInvoice
{
    public class GetReceiptInvoiceByIdResponse : BaseResponse
    {
        public ReceiptInvoiceModel ReceiptInvoice { get; set; }
        //public string ReceiptName { get; set; }
        //public string CreatedByName { get; set; }
    }
}
