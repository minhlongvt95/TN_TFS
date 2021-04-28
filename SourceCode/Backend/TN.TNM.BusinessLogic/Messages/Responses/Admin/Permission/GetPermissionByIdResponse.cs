using TN.TNM.BusinessLogic.Models.Admin;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Permission
{
    public class GetPermissionByIdResponse : BaseResponse
    {
        //public List<PermissionModel> PermissionList { get; set; }
        public PermissionSetModel Permission { get; set; }
    }
}


