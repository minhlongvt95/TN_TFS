using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Organization;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Organization
{
    public class GetChildrenByOrganizationIdRequest : BaseRequest<GetChildrenByOrganizationIdParameter>
    {
        public Guid OrganizationId { get; set; }
        public override GetChildrenByOrganizationIdParameter ToParameter()
        {
            {
                return new GetChildrenByOrganizationIdParameter
                {
                    OrganizationId = OrganizationId,
                    UserId = UserId
                };
            }
        }
    }
}
