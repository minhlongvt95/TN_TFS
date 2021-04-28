using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.BankBook
{
    public class SearchBankBookParameter : BaseParameter
    {
        public DateTime? ToPaidDate { get; set; }
        public DateTime? FromPaidDate { get; set; }
        public List<Guid> BankAccountId { get; set; }
    }
}
