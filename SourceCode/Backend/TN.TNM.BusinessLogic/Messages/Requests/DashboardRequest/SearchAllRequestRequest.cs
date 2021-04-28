using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.DashboardRequest;

namespace TN.TNM.BusinessLogic.Messages.Requests.DashboardRequest
{
    public class SearchAllRequestRequest : BaseRequest<SearchAllRequestParameter>
    {
        public List<string> ListSearchTypeRequest { get; set; }
        public override SearchAllRequestParameter ToParameter() => new SearchAllRequestParameter()
        {
            ListSearchTypeRequest = ListSearchTypeRequest,
            UserId = UserId
        };
    }
}
