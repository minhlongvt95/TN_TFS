using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Permission;
using TN.TNM.DataAccess.Models.MenuBuild;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Permission
{
    public class EditRoleAndPermissionRequest : BaseRequest<EditRoleAndPermissionParameter>
    {
        public Guid RoleId { get; set; }
        public string RoleValue { get; set; }
        public string Description { get; set; }
        public List<string> ListActionResource { get; set; }
        public List<MenuBuildEntityModel> ListMenuBuild { get; set; }

        public override EditRoleAndPermissionParameter ToParameter()
        {
            return new EditRoleAndPermissionParameter()
            {
                UserId = UserId,
                RoleId = RoleId,
                RoleValue = RoleValue,
                Description = Description,
                ListActionResource = ListActionResource,
                ListMenuBuild = ListMenuBuild
            };
        }
    }
}
