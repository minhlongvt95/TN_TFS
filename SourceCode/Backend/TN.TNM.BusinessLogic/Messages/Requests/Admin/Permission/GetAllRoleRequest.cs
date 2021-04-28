using TN.TNM.DataAccess.Messages.Parameters.Admin.Permission;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Permission
{
    public class GetAllRoleRequest : BaseRequest<GetAllRoleParameter>
    {
        public override GetAllRoleParameter ToParameter()
        {
            return new GetAllRoleParameter
            {
                UserId = UserId
            };
        }
    }
}
