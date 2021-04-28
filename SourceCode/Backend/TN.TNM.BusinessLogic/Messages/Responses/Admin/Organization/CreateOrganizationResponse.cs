using System;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Organization
{
    public class CreateOrganizationResponse : BaseResponse
    {
        public Guid CreatedOrgId { get; set; }
    }
}
