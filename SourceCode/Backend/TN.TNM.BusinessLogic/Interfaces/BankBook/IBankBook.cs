using TN.TNM.BusinessLogic.Messages.Requests.BankBook;
using TN.TNM.BusinessLogic.Messages.Responses.BankBook;

namespace TN.TNM.BusinessLogic.Interfaces.BankBook
{
    public interface IBankBook
    {
        SearchBankBookResponse SearchBankBook(SearchBankBookRequest request);
        GetMasterDataSearchBankBookResponse GetMasterDataSearchBankBook(GetMasterDataSearchBankBookRequest request);
    }
}
