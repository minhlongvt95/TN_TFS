using System;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Permission;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Permission
{
    public class DeletePermissionByIdRequest : BaseRequest<DeletePermissionByIdParameter>
    {
        public Guid PermissionSetId { get; set; }
        public override DeletePermissionByIdParameter ToParameter()
        {
            return new DeletePermissionByIdParameter() {
                UserId = UserId,
                PermissionSetId = PermissionSetId
            };
        }
    }
}
