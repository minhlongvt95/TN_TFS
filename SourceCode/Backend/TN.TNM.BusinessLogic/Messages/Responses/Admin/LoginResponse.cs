using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.MenuBuild;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin
{
    public class LoginResponse: BaseResponse
    {
        public AuthModel CurrentUser { get; set; }
        public string UserFullName { get; set; }
        public string UserAvatar { get; set; }
        public string UserEmail { get; set; }
        public bool IsManager { get; set; }
        public Guid? PositionId { get; set; }
        public List<string> PermissionList { get; set; }
        public List<string> ListPermissionResource { get; set; }
        public bool IsAdmin { get; set; }
        public List<SystemParameter> SystemParameterList { get; set; }
        public bool IsOrder { get; set; }
        public bool IsCashier { get; set; }
        public List<MenuBuildEntityModel> ListMenuBuild { get; set; }
    }
}
