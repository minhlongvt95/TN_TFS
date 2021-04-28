using System;

namespace TN.TNM.DataAccess.Messages.Parameters.BankAccount
{
    public class DeleteBankAccountByIdParameter : BaseParameter
    {
        public Guid BankAccountId { get; set; }
        public Guid ObjectId { get; set; }
        public string ObjectType { get; set; }
    }
}
