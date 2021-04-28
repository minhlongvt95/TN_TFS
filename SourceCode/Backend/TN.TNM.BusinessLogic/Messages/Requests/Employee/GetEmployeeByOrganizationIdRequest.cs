using System;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetEmployeeByOrganizationIdRequest : BaseRequest<GetEmployeeByOrganizationIdParameter>
    {
        public Guid OrganizationId { get; set; }

        public override GetEmployeeByOrganizationIdParameter ToParameter()
        {
            return new GetEmployeeByOrganizationIdParameter()
            {
                OrganizationId = OrganizationId
            };
        }
    }
}
