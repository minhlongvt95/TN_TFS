using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Organization
{
    public class GetOrganizationByIdParameter : BaseParameter
    {
        public Guid OrganizationId { get; set; }
    }
}
