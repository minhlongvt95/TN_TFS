using System;
using TN.TNM.DataAccess.Messages.Parameters.BankAccount;

namespace TN.TNM.BusinessLogic.Messages.Requests.BankAccount
{
    public class GetBankAccountByIdRequest : BaseRequest<GetBankAccountByIdParameter>
    {
        public Guid BankAccountId { get; set; }
        public override GetBankAccountByIdParameter ToParameter()
        {
            return new GetBankAccountByIdParameter() {
                UserId = UserId,
                BankAccountId = BankAccountId
            };
        }
    }
}
