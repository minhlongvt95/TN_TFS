using TN.TNM.DataAccess.Messages.Parameters.Admin.Permission;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Permission
{
    public class GetAllPermissionSetNameAndCodeRequest : BaseRequest<GetAllPermissionSetNameAndCodeParameter>
    {
        public override GetAllPermissionSetNameAndCodeParameter ToParameter()
        {
            return new GetAllPermissionSetNameAndCodeParameter()
            {
                UserId = UserId
            };
        }
    }
}
