using TN.TNM.DataAccess.Messages.Parameters.Admin.Permission;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Permission
{
    public class GetAllPermissionRequest : BaseRequest<GetAllPermissionParameter>
    {
        public override GetAllPermissionParameter ToParameter()
        {
            return new GetAllPermissionParameter {
                UserId = UserId
            };
        }
    }
}
