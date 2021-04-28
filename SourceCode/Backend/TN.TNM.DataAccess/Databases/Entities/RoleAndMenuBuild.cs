using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class RoleAndMenuBuild
    {
        public Guid RoleAndMenuBuildId { get; set; }
        public Guid MenuBuildId { get; set; }
        public Guid RoleId { get; set; }
        public string Code { get; set; }
        public string Path { get; set; }
        public Guid? TenantId { get; set; }
    }
}
