using System;

namespace TN.TNM.DataAccess.Messages.Parameters.BankAccount
{
    public class GetBankAccountByIdParameter : BaseParameter
    {
        public Guid BankAccountId { get; set; }
    }
}
