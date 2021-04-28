using TN.TNM.DataAccess.Messages.Parameters.Admin.Organization;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Organization
{
    public class GetAllOrganizationCodeRequest : BaseRequest<GetAllOrganizationCodeParameter>
    {
        public override GetAllOrganizationCodeParameter ToParameter()
        {
            return new GetAllOrganizationCodeParameter()
            {
                UserId = UserId
            };
        }
    }
}
