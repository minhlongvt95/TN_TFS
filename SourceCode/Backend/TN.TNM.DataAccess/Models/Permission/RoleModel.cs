using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.Permission
{
    public class RoleModel : Role
    {
        public int UserNumber { get; set; }
    }
}
