using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Permission
{
    public class DeletePermissionByIdParameter : BaseParameter
    {
        public Guid PermissionSetId { get; set; }
    }
}
