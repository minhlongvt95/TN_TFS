using System.Collections.Generic;
using TN.TNM.DataAccess.Models.MenuBuild;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Permission
{
    public class CreateRoleAndPermissionParameter : BaseParameter
    {
        public string RoleValue { get; set; }
        public string Description { get; set; }
        public List<string> ListActionResource { get; set; }
        public List<MenuBuildEntityModel> ListMenuBuild { get; set; }
    }
}
