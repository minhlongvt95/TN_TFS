using System;
using TN.TNM.DataAccess.Messages.Parameters.BankAccount;

namespace TN.TNM.BusinessLogic.Messages.Requests.BankAccount
{
    public class GetAllBankAccountByObjectRequest : BaseRequest<GetAllBankAccountByObjectParameter>
    {
        public Guid ObjectId { get; set; }
        public string ObjectType { get; set; }
        public override GetAllBankAccountByObjectParameter ToParameter()
        {
            return new GetAllBankAccountByObjectParameter
            {
                UserId = UserId,
                ObjectId = ObjectId,
                ObjectType = ObjectType
            };
        }
    }
}
