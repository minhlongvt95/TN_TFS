using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Lead;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class GetPotentialRateResponse : BaseResponse
    {
        public int TotalCount { get; set; }
        public List<LeadPotentialRateModel> LeadPotentialRateList { get; set; }
    }
    
}
