using TN.TNM.BusinessLogic.Models.BankAccount;
using TN.TNM.DataAccess.Messages.Parameters.BankAccount;

namespace TN.TNM.BusinessLogic.Messages.Requests.BankAccount
{
    public class EditBankAccountRequest : BaseRequest<EditBankAccountParameter>
    {
        public BankAccountModel BankAccount { get; set; }
        public override EditBankAccountParameter ToParameter()
        {
            return new EditBankAccountParameter()
            {
                UserId = UserId,
                BankAccount = BankAccount.ToEntity()
            };
        }
    }
}
