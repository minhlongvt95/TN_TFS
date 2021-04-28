using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Permission;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Permission
{
    public class GetPermissionByIdRequest : BaseRequest<GetPermissionByIdParameter>
    {
        public Guid PermissionSetId { get; set; }
        public override GetPermissionByIdParameter ToParameter()
        {
            return new GetPermissionByIdParameter()
            {
                UserId = UserId,
                PermissionSetId = PermissionSetId
            };
        }
    }
}
