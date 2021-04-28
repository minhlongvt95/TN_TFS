using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Organization
{
    public class GetChildrenOrganizationByIdParameter : BaseParameter
    {
        public Guid? EmployeeId { get; set; }
    }
}
