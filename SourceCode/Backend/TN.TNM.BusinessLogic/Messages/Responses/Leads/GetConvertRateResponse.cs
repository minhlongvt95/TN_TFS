using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Lead;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class GetConvertRateResponse : BaseResponse
    {
        public int TotalCount { get; set; }
        public int TotalCountRequirementRate { get; set; }
        public int TotalCountPotentialRate { get; set; }
        public List<LeadConvertRateModel> LeadConvertRateList { get; set; }
        public List<LeadRequirementRateModel> LeadRequirementRateList { get; set; }
        public List<LeadPotentialRateModel> LeadPotentialRateList { get; set; }
        public List<LeadModel> ListMOILead { get; set; }
        public List<LeadModel> ListCHOLead { get; set; }
        public List<LeadModel> ListNDOLead { get; set; }
    }
}
