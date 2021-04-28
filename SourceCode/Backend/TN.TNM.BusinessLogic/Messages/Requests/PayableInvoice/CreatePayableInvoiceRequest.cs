using TN.TNM.BusinessLogic.Models.PayableInvoice;
using TN.TNM.DataAccess.Messages.Parameters.PayableInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.PayableInvoice
{
    public class CreatePayableInvoiceRequest : BaseRequest<CreatePayableInvoiceParameter>
    {
        public PayableInvoiceModel PayableInvoice { get; set; }
        public PayableInvoiceMappingModel PayableInvoiceMapping { get; set; }

        public override CreatePayableInvoiceParameter ToParameter() 
        {
            return new CreatePayableInvoiceParameter
            {
                PayableInvoice = PayableInvoice.ToEntity(),
                PayableInvoiceMapping = PayableInvoiceMapping.ToEntity(),
                UserId = UserId,
            };
        }
    }
}
