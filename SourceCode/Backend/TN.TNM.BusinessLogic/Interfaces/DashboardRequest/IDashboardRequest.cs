using TN.TNM.BusinessLogic.Messages.Requests.DashboardRequest;
using TN.TNM.BusinessLogic.Messages.Responses.DashboardRequest;

namespace TN.TNM.BusinessLogic.Interfaces.DashboardRequest
{
    public interface IDashboardRequest
    {
        GetAllRequestResponse GetAllRequest(GetAllRequestRequest request);
        SearchAllRequestResponse SearchAllRequest(SearchAllRequestRequest request);
    }
}
