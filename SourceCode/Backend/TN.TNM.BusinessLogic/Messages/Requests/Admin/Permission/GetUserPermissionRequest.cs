using TN.TNM.DataAccess.Messages.Parameters.Admin.Permission;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Permission
{
    public class GetUserPermissionRequest : BaseRequest<GetUserPermissionParameter>
    {
        public override GetUserPermissionParameter ToParameter()
        {
            return new GetUserPermissionParameter()
            {
                UserId = UserId
            };
        }
    }
}
