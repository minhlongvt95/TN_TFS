using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.DashboardRequest
{
    public class SearchAllRequestParameter : BaseParameter
    {
        public List<string> ListSearchTypeRequest { get; set; }
    }
}
