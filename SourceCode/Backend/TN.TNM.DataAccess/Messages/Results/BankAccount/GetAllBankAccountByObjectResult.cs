using System.Collections.Generic;
using TN.TNM.DataAccess.Models.BankAccount;

namespace TN.TNM.DataAccess.Messages.Results.BankAccount
{
    public class GetAllBankAccountByObjectResult : BaseResult
    {
        public List<BankAccountEntityModel> BankAccountList { get; set; }
    }
}
