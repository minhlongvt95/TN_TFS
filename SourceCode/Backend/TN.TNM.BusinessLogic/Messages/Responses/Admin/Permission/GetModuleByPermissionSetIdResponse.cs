using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Admin;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Permission
{
    public class GetModuleByPermissionSetIdResponse : BaseResponse
    {
        public List<PermissionModel> PermissionListAsModule { get; set; }
    }
}
