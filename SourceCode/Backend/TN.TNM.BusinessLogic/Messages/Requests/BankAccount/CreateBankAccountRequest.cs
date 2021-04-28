using System;
using TN.TNM.BusinessLogic.Models.BankAccount;
using TN.TNM.DataAccess.Messages.Parameters.BankAccount;
using TN.TNM.DataAccess.Models.BankAccount;

namespace TN.TNM.BusinessLogic.Messages.Requests.BankAccount
{
    public class CreateBankAccountRequest : BaseRequest<CreateBankAccountParameter>
    {
        public BankAccountModel BankAccount { get; set; }
        public override CreateBankAccountParameter ToParameter()
        {
            var _bankAccount = new BankAccountEntityModel();
            _bankAccount.BankAccountId = BankAccount.BankAccountId;
            _bankAccount.ObjectId = BankAccount.ObjectId;
            _bankAccount.ObjectType = BankAccount.ObjectType;
            _bankAccount.AccountNumber = BankAccount.AccountNumber;
            _bankAccount.BankName = BankAccount.BankName;
            _bankAccount.BankDetail = BankAccount.BankDetail;
            _bankAccount.BranchName = BankAccount.BranchName;
            _bankAccount.AccountName = BankAccount.AccountName;
            _bankAccount.Active = true;
            _bankAccount.CreatedById = BankAccount.CreatedById;
            _bankAccount.CreatedDate = BankAccount.CreatedDate;
            _bankAccount.UpdatedById = BankAccount.UpdatedById;
            _bankAccount.UpdatedDate = BankAccount.UpdatedDate;

            return new CreateBankAccountParameter() {
                UserId = UserId,
                BankAccount = _bankAccount
            };
        }
    }
}
