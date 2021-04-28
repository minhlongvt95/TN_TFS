namespace TN.TNM.DataAccess.Messages.Results.BankAccount
{
    public class GetBankAccountByIdResult : BaseResult
    {
        public Databases.Entities.BankAccount BankAccount { get; set; }
    }
}
