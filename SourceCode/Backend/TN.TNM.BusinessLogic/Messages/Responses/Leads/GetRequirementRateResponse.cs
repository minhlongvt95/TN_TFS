using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Lead;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class GetRequirementRateResponse : BaseResponse
    {
        public int TotalCount { get; set; }
        public List<LeadRequirementRateModel> LeadRequirementRateList { get; set; }
    }
   
}
