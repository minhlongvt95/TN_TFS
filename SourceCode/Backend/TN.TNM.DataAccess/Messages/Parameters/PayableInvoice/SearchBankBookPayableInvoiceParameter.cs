using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.PayableInvoice
{
    public class SearchBankBookPayableInvoiceParameter : BaseParameter
    {
        public DateTime? ToPaidDate { get; set; }
        public DateTime? FromPaidDate { get; set; }
        public List<Guid> BankAccountId { get; set; }
        public List<Guid> ListCreateById { get; set; }
    }
}
