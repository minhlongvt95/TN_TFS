using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    
    public class GetRequirementRateResult : BaseResult
    {
        public int TotalCount { get; set; }
        public List<LeadRequirementRateEntityModel> LeadRequirementRateList { get; set; }
    }
}
