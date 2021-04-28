using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.BankAccount
{
    public class CreateBankAccountResult : BaseResult
    {
        public List<Databases.Entities.BankAccount> ListBankAccount { get; set; }
    }
}
