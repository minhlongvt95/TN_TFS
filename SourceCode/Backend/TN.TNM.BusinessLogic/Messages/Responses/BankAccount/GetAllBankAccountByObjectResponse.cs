using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.BankAccount;
using TN.TNM.DataAccess.Models.BankAccount;

namespace TN.TNM.BusinessLogic.Messages.Responses.BankAccount
{
    public class GetAllBankAccountByObjectResponse : BaseResponse {
        public List<BankAccountEntityModel> BankAccountList { get; set; }
    }
}
