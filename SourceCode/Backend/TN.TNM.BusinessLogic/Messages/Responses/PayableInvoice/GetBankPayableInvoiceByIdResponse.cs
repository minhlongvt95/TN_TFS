using TN.TNM.BusinessLogic.Models.PayableInvoice;

namespace TN.TNM.BusinessLogic.Messages.Responses.PayableInvoice
{
    public class GetBankPayableInvoiceByIdResponse : BaseResponse
    {
        public BankPayableInvoiceModel BankPayableInvoice { get; set; }

    }
}
