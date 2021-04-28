using System.Collections.Generic;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Permission
{
    public class GetModuleByPermissionSetIdResult : BaseResult
    {
        public List<PermissionEntityModel> PermissionListAsModule { get; set; }
    }
}
