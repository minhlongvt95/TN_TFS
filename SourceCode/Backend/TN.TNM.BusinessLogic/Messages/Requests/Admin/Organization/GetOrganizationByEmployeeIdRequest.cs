using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Organization;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Organization
{
    public class GetOrganizationByEmployeeIdRequest : BaseRequest<GetOrganizationByEmployeeIdParameter>
    {
        public Guid EmployeeId { get; set; }
        public override GetOrganizationByEmployeeIdParameter ToParameter()
        {
            return new GetOrganizationByEmployeeIdParameter
            {
                EmployeeId = EmployeeId,
                UserId = UserId
            };
        }
    }
}
