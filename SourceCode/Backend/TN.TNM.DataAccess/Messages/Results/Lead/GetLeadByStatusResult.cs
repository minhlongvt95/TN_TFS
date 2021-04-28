using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class GetLeadByStatusResult : BaseResult
    {
        public List<LeadEntityModel> ListLead { get; set; }
    }
}
