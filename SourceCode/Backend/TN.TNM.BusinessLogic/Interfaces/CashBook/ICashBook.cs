using TN.TNM.BusinessLogic.Messages.Requests.CashBook;
using TN.TNM.BusinessLogic.Messages.Responses.CashBook;

namespace TN.TNM.BusinessLogic.Interfaces.CashBook
{
    public interface ICashBook
    {
        GetSurplusCashBookPerMonthResponse GetSurplusCashBookPerMonth(GetSurplusCashBookPerMonthRequest request);
        GetDataSearchCashBookResponse GetDataSearchCashBook(GetDataSearchCashBookRequest request);
    }
}
