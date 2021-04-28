using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Lead;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class GetAllLeadResponse : BaseResponse
    {
        public List<GetAllLeadModel> ListLead { get; set; }
    }
}
