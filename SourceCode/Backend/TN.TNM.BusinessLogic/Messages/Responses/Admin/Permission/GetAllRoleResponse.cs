using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Admin;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Permission
{
    public class GetAllRoleResponse : BaseResponse
    {
        public List<RoleModel> ListRole { get; set; }
    }
}
