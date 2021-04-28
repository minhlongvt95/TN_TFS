using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.PayableInvoice
{
    public class CreateBankPayableInvoiceParameter : BaseParameter
    {
        public BankPayableInvoice BankPayableInvoice { get; set; }
        public BankPayableInvoiceMapping BankPayableInvoiceMapping { get; set; }
    }
}
