using System;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Organization
{
    public class CreateOrganizationResult : BaseResult
    {
        public Guid CreatedOrgId { get; set; }
    }
}
