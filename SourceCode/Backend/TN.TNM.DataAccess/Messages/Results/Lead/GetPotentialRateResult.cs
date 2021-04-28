using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class GetPotentialRateResult: BaseResult
    {
        public int TotalCount { get; set; }
        public List<LeadPotentialRateEntityModel> LeadPotentialRateList { get; set; }
    }
    
}
