using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.BankAccount
{
    public class GetCompanyBankAccountResult : BaseResult
    {
        public List<Databases.Entities.BankAccount> BankList { get; set; }
    }
}
