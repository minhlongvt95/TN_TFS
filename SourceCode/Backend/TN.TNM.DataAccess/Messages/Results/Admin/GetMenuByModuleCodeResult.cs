using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Admin
{
    public class GetMenuByModuleCodeResult: BaseResult
    {
        public ICollection<Databases.Entities.Permission> Permissions { get; set; }
        //public List<PermissionEntityModel> Permissions { get; set; }
    }
}
