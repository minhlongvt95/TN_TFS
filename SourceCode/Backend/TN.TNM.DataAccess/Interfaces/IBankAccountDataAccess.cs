using TN.TNM.DataAccess.Messages.Parameters.BankAccount;
using TN.TNM.DataAccess.Messages.Results.BankAccount;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IBankAccountDataAccess
    {
        CreateBankAccountResult CreateBankAccount(CreateBankAccountParameter parameter);
        GetBankAccountByIdResult GetBankAccountById(GetBankAccountByIdParameter parameter);
        EditBankAccountResult EditBankAccount(EditBankAccountParameter parameter);
        DeleteBankAccountByIdResult DeleteBankAccountById(DeleteBankAccountByIdParameter parameter);
        GetAllBankAccountByObjectResult GetAllBankAccountByObject(GetAllBankAccountByObjectParameter parameter);
        GetCompanyBankAccountResult GetCompanyBankAccount(GetCompanyBankAccountParameter parameter);
    }
}
