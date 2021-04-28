using System;

namespace TN.TNM.DataAccess.Messages.Parameters.PayableInvoice
{
    public class GetBankPayableInvoiceByIdParameter : BaseParameter
    {
        public Guid BankPayableInvoiceId { get; set; }
    }
}
