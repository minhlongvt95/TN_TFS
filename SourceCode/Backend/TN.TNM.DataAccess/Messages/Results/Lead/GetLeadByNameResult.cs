using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class GetLeadByNameResult :BaseResult
    {
        public List<LeadEntityModel> ListLead { get; set; }
    }
}
