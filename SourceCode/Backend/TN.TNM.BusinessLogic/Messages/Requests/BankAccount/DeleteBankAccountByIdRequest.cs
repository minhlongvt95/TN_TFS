using System;
using TN.TNM.DataAccess.Messages.Parameters.BankAccount;

namespace TN.TNM.BusinessLogic.Messages.Requests.BankAccount
{
    public class DeleteBankAccountByIdRequest : BaseRequest<DeleteBankAccountByIdParameter>
    {
        public Guid BankAccountId { get; set; }
        public Guid ObjectId { get; set; }
        public string ObjectType { get; set; }
        public override DeleteBankAccountByIdParameter ToParameter()
        {
            return new DeleteBankAccountByIdParameter()
            {
                UserId = UserId,
                BankAccountId = BankAccountId,
                ObjectId = ObjectId,
                ObjectType = ObjectType
            };
        }
    }
}
