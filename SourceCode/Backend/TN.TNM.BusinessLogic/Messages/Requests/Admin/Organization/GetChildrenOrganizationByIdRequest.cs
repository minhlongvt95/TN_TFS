using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Organization;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Organization
{
    public class GetChildrenOrganizationByIdRequest : BaseRequest<GetChildrenOrganizationByIdParameter>
    {
        public Guid? EmployeeId { get; set; }

        public override GetChildrenOrganizationByIdParameter ToParameter()
        {
            return new GetChildrenOrganizationByIdParameter()
            {
                EmployeeId = EmployeeId
            };
        }
    }
}
