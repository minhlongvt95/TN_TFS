using System;

namespace TN.TNM.DataAccess.Messages.Parameters.BankAccount
{
    public class GetAllBankAccountByObjectParameter : BaseParameter
    {
        public Guid ObjectId { get; set; }
        public string ObjectType { get; set; }
    }
}
