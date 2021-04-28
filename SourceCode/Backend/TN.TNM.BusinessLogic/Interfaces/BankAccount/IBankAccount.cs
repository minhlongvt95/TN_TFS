using TN.TNM.BusinessLogic.Messages.Requests.BankAccount;
using TN.TNM.BusinessLogic.Messages.Responses.BankAccount;

namespace TN.TNM.BusinessLogic.Interfaces.BankAccount
{
    public interface IBankAccount
    {
        CreateBankAccountResponse CreateBankAccount(CreateBankAccountRequest request);
        GetBankAccountByIdResponse GetBankAccountById(GetBankAccountByIdRequest request);
        GetAllBankAccountByObjectResponse GetAllBankAccountByObject(GetAllBankAccountByObjectRequest request);
        EditBankAccountResponse EditBankAccount(EditBankAccountRequest request);
        DeleteBankAccountByIdResponse DeleteBankAccount(DeleteBankAccountByIdRequest request);
        GetCompanyBankAccountResponse GetCompanyBankAccount(GetCompanyBankAccountRequest request);
    }
}
