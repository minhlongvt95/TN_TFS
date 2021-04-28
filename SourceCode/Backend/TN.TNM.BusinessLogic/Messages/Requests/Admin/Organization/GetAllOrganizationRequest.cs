using TN.TNM.DataAccess.Messages.Parameters.Admin.Organization;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Organization
{
    public class GetAllOrganizationRequest : BaseRequest<GetAllOrganizationParameter>
    {
        public override GetAllOrganizationParameter ToParameter() => new GetAllOrganizationParameter()
        {

        };
    }
}
