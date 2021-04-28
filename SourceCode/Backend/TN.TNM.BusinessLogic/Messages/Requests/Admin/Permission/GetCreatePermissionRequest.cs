using TN.TNM.DataAccess.Messages.Parameters.Admin.Permission;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Permission
{
    public class GetCreatePermissionRequest : BaseRequest<GetCreatePermissionParameter>
    {
        public override GetCreatePermissionParameter ToParameter()
        {
            return new GetCreatePermissionParameter
            {
                UserId = UserId
            };
        }
    }
}
