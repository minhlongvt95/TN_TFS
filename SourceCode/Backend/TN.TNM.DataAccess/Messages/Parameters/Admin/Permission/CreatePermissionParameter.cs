using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Permission
{
    public class CreatePermissionParameter : BaseParameter
    {
        public string PermissionSetName { get; set; }
        public string PermissionSetCode { get; set; }
        public string PermissionSetDescription { get; set; }
        public List<string> PermissionIdList { get; set; }
    }
}
