using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Organization;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Organization
{
    public class GetOrganizationByIdRequest : BaseRequest<GetOrganizationByIdParameter>
    {
        public Guid OrganizationId { get; set; }
        public override GetOrganizationByIdParameter ToParameter()
        {
            return new GetOrganizationByIdParameter()
            {
                OrganizationId = OrganizationId,
                UserId = UserId
            };
        }
    }
}
