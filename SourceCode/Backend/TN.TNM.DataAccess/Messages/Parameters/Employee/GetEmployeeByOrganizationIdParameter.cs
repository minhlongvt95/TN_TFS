using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetEmployeeByOrganizationIdParameter : BaseParameter
    {
        public Guid OrganizationId { get; set; }
    }
}
