using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class GetConvertRateResult : BaseResult
    {
        public int TotalCount { get; set; }
        public int TotalCountRequirementRate { get; set; }
        public int TotalCountPotentialRate { get; set; }
        public List<LeadConvertRateEntityModel> LeadConvertRateList { get; set; }
        public List<LeadRequirementRateEntityModel> LeadRequirementRateList { get; set; }
        public List<LeadPotentialRateEntityModel> LeadPotentialRateList { get; set; }
        public List<LeadEntityModel> ListMOILead { get; set; }
        public List<LeadEntityModel> ListNDOLead { get; set; }
        public List<LeadEntityModel> ListCHOLead { get; set; }
    }
    
}
