using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.DashboardRequest
{
    public class SearchAllRequestResult : BaseResult
    {
        public List<RequestDetail> RequestList { get; set; }
    }

}
