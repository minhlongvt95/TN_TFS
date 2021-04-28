using System;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class EditLeadStatusByIdRequest : BaseRequest<EditLeadStatusByIdParameter>
    {
        public Guid LeadId { get; set; }
        public string LeadStatusCode { get; set; }
        public override EditLeadStatusByIdParameter ToParameter()
        {
            return new EditLeadStatusByIdParameter
            {
                LeadId = LeadId,
                LeadStatusCode = LeadStatusCode
            };
        }
    }
}
