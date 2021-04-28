using TN.TNM.BusinessLogic.Models.PayableInvoice;
using TN.TNM.DataAccess.Messages.Parameters.PayableInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.PayableInvoice
{
    public class EditPayableInvoiceRequest : BaseRequest<EditPayableInvoiceParameter>
    {
        public PayableInvoiceModel PayableInvoice { get; set; }

        public override EditPayableInvoiceParameter ToParameter() => new EditPayableInvoiceParameter()
        {
            PayableInvoice = PayableInvoice.ToEntity()
        };
    }
}
