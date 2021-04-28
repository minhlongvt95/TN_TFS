using System.Collections.Generic;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Permission
{
    public class GetUserPermissionResponse : BaseResponse
    {
        public List<string> PermissionList { get; set; }
    }
}
