using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Admin;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Permission
{
    public class GetPermissionByCodeResponse : BaseResponse
    {
        public List<PermissionModel> PermissionList { get; set; }
    }
}
