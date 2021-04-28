using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Organization
{
    public class DeleteOrganizationByIdParameter : BaseParameter
    {
        public Guid OrganizationId { get; set; }
    }
}
