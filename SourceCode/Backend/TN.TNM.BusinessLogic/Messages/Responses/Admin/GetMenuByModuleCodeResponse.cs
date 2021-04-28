using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Admin;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin
{
    public class GetMenuByModuleCodeResponse : BaseResponse
    {
        public List<PermissionModel> Permissions { get; set; }
    }
}
