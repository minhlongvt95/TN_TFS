using System;

namespace TN.TNM.DataAccess.Messages.Parameters.PayableInvoice
{
    public class ExportBankPayableInvoiceParameter : BaseParameter
    {
        public Guid BankPayableInvId { get; set; }
    }
}
