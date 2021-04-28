using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Permission
{
    public class GetAllPermissionSetNameAndCodeResult : BaseResult
    {
        public List<string> PermissionSetNameList { get; set; }
        public List<string> PermissionSetCodeList { get; set; }
    }
}
