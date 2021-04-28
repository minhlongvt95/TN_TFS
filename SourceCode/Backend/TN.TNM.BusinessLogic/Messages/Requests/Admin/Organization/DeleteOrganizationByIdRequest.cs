using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Organization;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Organization
{
    public class DeleteOrganizationByIdRequest : BaseRequest<DeleteOrganizationByIdParameter>
    {
        public Guid OrganizationId { get; set; }

        public override DeleteOrganizationByIdParameter ToParameter()
        {
            return new DeleteOrganizationByIdParameter()
            {
                OrganizationId = OrganizationId,
                UserId = UserId
            };
        }
    }
}
