using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Organization
{
    public class GetOrganizationByEmployeeIdParameter : BaseParameter
    {
        public Guid? EmployeeId { get; set; }
    }
}
