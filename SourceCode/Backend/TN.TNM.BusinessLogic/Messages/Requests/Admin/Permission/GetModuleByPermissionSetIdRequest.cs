using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Permission;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Permission
{
    public class GetModuleByPermissionSetIdRequest : BaseRequest<GetModuleByPermissionSetIdParameter>
    {
        public Guid PermissionSetId { get; set; }
        public override GetModuleByPermissionSetIdParameter ToParameter()
        {
            return new GetModuleByPermissionSetIdParameter() {
                UserId = UserId,
                PermissionSetId = PermissionSetId
            };
        }
    }
}
