using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class GetDataLeadDashboardResponse: BaseResponse
    {
        public DataAccess.Models.Lead.LeadDashBoardEntityModel LeadDashBoard { get; set; }
    }
}
