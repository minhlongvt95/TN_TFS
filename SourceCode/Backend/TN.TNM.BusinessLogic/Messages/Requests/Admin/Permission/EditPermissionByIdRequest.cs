using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Permission;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Permission
{
    public class EditPermissionByIdRequest : BaseRequest<EditPermissionByIdParameter>
    {
        public Guid PermissionSetId { get; set; }
        public string PermissionSetName { get; set; }
        public string PermissionSetDescription { get; set; }
        public string PermissionSetCode { get; set; }
        public List<string> PermissionIdList { get; set; }
        public override EditPermissionByIdParameter ToParameter()
        {
            return new EditPermissionByIdParameter() {
                UserId = UserId,
                PermissionSetId = PermissionSetId,
                PermissionSetDescription = PermissionSetDescription,
                PermissionSetName = PermissionSetName,
                PermissionSetCode = PermissionSetCode,
                PermissionIdList = PermissionIdList
            };
        }
    }
}
