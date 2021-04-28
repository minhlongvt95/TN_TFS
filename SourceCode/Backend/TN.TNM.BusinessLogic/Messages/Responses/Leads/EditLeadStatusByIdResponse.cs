using System;

namespace TN.TNM.BusinessLogic.Messages.Responses.Leads
{
    public class EditLeadStatusByIdResponse: BaseResponse
    {
        public Guid LeadId { get; set; }
    }
}
