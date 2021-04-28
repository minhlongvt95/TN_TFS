using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.BankAccount;

namespace TN.TNM.BusinessLogic.Messages.Responses.BankAccount
{
    public class DeleteBankAccountByIdResponse : BaseResponse
    {
        public List<BankAccountModel> ListBankAccount { get; set; }
    }
}
