using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.MenuBuild;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Permission
{
    public class GetDetailPermissionResponse : BaseResponse
    {
        public List<ActionResourceModel> ListActionResource { get; set; }
        public List<ActionResourceModel> ListCurrentActionResource { get; set; }
        public Role Role { get; set; }
        public List<MenuBuildEntityModel> ListMenuBuild { get; set; }
    }
}
