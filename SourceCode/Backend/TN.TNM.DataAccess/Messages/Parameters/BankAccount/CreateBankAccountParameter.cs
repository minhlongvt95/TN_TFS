using TN.TNM.DataAccess.Models.BankAccount;

namespace TN.TNM.DataAccess.Messages.Parameters.BankAccount
{
    public class CreateBankAccountParameter : BaseParameter
    {
        public BankAccountEntityModel BankAccount {get;set;}
    }
}
