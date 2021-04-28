using TN.TNM.DataAccess.Messages.Parameters.BankAccount;

namespace TN.TNM.BusinessLogic.Messages.Requests.BankAccount
{
    public class GetCompanyBankAccountRequest : BaseRequest<GetCompanyBankAccountParameter>
    {
        public override GetCompanyBankAccountParameter ToParameter()
        {
            return new GetCompanyBankAccountParameter()
            {
                UserId = UserId
            };
        }
    }
}
