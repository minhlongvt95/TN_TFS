using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.MenuBuild;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Permission
{
    public class GetDetailPermissionResult : BaseResult
    {
        public List<ActionResource> ListActionResource { get; set; }
        public List<ActionResource> ListCurrentActionResource { get; set; }
        public Role Role { get; set; }
        public List<MenuBuildEntityModel> ListMenuBuild { get; set; }
    }
}
