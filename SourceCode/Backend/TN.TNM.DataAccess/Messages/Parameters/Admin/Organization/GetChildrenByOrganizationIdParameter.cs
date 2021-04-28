using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Organization
{
    public class GetChildrenByOrganizationIdParameter : BaseParameter
    {
        public Guid OrganizationId { get; set; }
    }
}
