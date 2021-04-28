using TN.TNM.DataAccess.Messages.Parameters.DashboardRequest;
using TN.TNM.DataAccess.Messages.Results.DashboardRequest;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IDashboardRequestDataAccess
    {
        GetAllRequestResult GetAllRequest(GetAllRequestParameter parameter);
        SearchAllRequestResult SearchAllRequest(SearchAllRequestParameter parameter);

    }
}
