using System.Collections.Generic;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Permission
{
    public class GetPermissionByCodeResult : BaseResult
    {
        public List<PermissionEntityModel> PermissionList { get; set; }
    }
}
