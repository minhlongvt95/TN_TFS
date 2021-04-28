using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Permission;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Permission
{
    public class CreatePermissionRequest : BaseRequest<CreatePermissionParameter>
    {
        public List<string> PermissionIdList { get; set; }
        public string PermissionSetName { get; set; }
        public string PermissionSetCode { get; set; }
        public string PermissionSetDescription { get; set; }
        public override CreatePermissionParameter ToParameter()
        {
            return new CreatePermissionParameter() {
                UserId = UserId,
                PermissionIdList = PermissionIdList,
                PermissionSetName = PermissionSetName,
                PermissionSetCode = PermissionSetCode,
                PermissionSetDescription = PermissionSetDescription
            };
        }
    }
}
