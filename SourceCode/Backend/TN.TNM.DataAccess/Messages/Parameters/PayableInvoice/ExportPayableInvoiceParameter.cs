using System;

namespace TN.TNM.DataAccess.Messages.Parameters.PayableInvoice
{
    public class ExportPayableInvoiceParameter : BaseParameter
    {
        public Guid PayableInvoiceId { get; set; }
    }
}
