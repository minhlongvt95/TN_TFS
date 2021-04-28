using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Permission
{
    public class EditPermissionByIdParameter : BaseParameter
    {
        public Guid PermissionSetId { get; set; }
        public string PermissionSetName { get; set; }
        public string PermissionSetDescription { get; set; }
        public string PermissionSetCode { get; set; }
        public List<string> PermissionIdList { get; set; }
    }
}
