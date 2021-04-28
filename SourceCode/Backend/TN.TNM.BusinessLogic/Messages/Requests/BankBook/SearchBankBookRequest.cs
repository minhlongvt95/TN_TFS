using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.BankBook;

namespace TN.TNM.BusinessLogic.Messages.Requests.BankBook
{
    public class SearchBankBookRequest : BaseRequest<SearchBankBookParameter>
    {
        public DateTime? ToPaidDate { get; set; }
        public DateTime? FromPaidDate { get; set; }
        public List<Guid> BankAccountId { get; set; }
        public override SearchBankBookParameter ToParameter()
        {
            return new SearchBankBookParameter() {
                UserId=this.UserId,
                ToPaidDate = ToPaidDate,
                FromPaidDate = FromPaidDate,
                BankAccountId = BankAccountId
            };
        }
    }
}
