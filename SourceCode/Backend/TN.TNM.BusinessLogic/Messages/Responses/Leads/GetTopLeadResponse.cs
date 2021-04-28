using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Lead;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class GetTopLeadResponse : BaseResponse
    {
        public List<LeadModel> ListLead { get; set; }
    }
}
