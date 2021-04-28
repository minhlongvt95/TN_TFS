using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.MenuBuild
{
    public class RoleAndMenuBuildEntityModel
    {
        public Guid? RoleAndMenuBuildId { get; set; }
        public Guid? MenuBuildId { get; set; }
        public Guid? RoleId { get; set; }
        public string Code { get; set; }
        public string Path { get; set; }
    }
}
