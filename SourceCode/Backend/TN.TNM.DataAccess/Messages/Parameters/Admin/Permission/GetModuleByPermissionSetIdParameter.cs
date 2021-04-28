using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Permission
{
    public class GetModuleByPermissionSetIdParameter : BaseParameter
    {
        public Guid PermissionSetId { get; set; }
    }
}
