using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Permission
{
    public class GetAllPermissionSetNameAndCodeResponse : BaseResponse
    {
        public List<string> PermissionSetNameList { get; set; }
        public List<string> PermissionSetCodeList { get; set; }
    }
}
