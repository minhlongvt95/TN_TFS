using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Permission
{
    public class GetPermissionByIdResult : BaseResult
    {
        //public List<PermissionEntityModel> PermissionList { get; set; }
        public PermissionSetEntityModel Permission { get; set; }
    }
}
