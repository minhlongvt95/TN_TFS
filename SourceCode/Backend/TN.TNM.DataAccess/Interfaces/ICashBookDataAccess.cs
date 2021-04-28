using TN.TNM.DataAccess.Messages.Parameters.CashBook;
using TN.TNM.DataAccess.Messages.Results.CashBook;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface ICashBookDataAccess
    {
        GetSurplusCashBookPerMonthResult GetSurplusCashBookPerMonth(GetSurplusCashBookPerMonthParameter parameter);
        GetDataSearchCashBookResult GetDataSearchCashBook(GetDataSearchCashBookParameter parameter);
    }
}
