using TN.TNM.BusinessLogic.Models.BankAccount;

namespace TN.TNM.BusinessLogic.Messages.Responses.BankAccount
{
    public class GetBankAccountByIdResponse : BaseResponse
    {
        public BankAccountModel BankAccount { get; set; }
    }
}
