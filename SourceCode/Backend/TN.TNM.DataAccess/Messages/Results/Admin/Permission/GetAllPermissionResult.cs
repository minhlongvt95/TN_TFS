using System.Collections.Generic;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Permission
{
    public class GetAllPermissionResult : BaseResult
    {
        public List<PermissionSetEntityModel> PermissionList { get; set; }
    }
}
