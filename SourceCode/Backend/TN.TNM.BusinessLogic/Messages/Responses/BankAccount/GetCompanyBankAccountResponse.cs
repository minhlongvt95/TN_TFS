using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.BankAccount;

namespace TN.TNM.BusinessLogic.Messages.Responses.BankAccount
{
    public class GetCompanyBankAccountResponse : BaseResponse
    {
        public List<BankAccountModel> BankList { get; set; }
    }
}
