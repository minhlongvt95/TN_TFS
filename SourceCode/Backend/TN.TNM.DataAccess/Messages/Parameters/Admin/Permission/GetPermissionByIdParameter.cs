using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Permission
{
    public class GetPermissionByIdParameter : BaseParameter
    {
        public Guid PermissionSetId { get; set; }
    }
}
