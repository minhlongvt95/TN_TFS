using TN.TNM.BusinessLogic.Models.PayableInvoice;
using TN.TNM.DataAccess.Messages.Parameters.PayableInvoice;

namespace TN.TNM.BusinessLogic.Messages.Requests.PayableInvoice
{
    public class CreateBankPayableInvoiceRequest : BaseRequest<CreateBankPayableInvoiceParameter>
    {
        public BankPayableInvoiceModel BankPayableInvoice { get; set; }
        public BankPayableInvoiceMappingModel BankPayableInvoiceMapping { get; set; }
        public override CreateBankPayableInvoiceParameter ToParameter()
        {
            return new CreateBankPayableInvoiceParameter() {
                UserId = UserId,
                BankPayableInvoice = BankPayableInvoice.ToEntity(),
                BankPayableInvoiceMapping = BankPayableInvoiceMapping.ToEntity()
            };
        }
    }
}
