using TN.TNM.DataAccess.Messages.Parameters.BankBook;
using TN.TNM.DataAccess.Messages.Results.BankBook;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IBankBookDataAccess
    {
        SearchBankBookResult SearchBankBook(SearchBankBookParameter parameter);
        GetMaterDataSearchBankBookResult GetMasterDataSearchBankBook(GetMasterDataSearchBankBookParameter parameter);
    }
}
