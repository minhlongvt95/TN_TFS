using System;

namespace TN.TNM.DataAccess.Messages.Parameters.PayableInvoice
{
    public class GetPayableInvoiceByIdParameter : BaseParameter
    {
        public Guid PayableInvoiceId { get; set; }
    }
}
