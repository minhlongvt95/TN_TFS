using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Admin;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Permission
{
    public class GetAllPermissionResponse : BaseResponse
    {
        public List<PermissionSetModel> PermissionList { get; set; }
    }
}
