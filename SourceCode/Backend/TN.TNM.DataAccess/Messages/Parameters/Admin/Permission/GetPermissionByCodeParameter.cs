using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Permission
{
    public class GetPermissionByCodeParameter : BaseParameter
    {
        public List<string> PerCode { get; set; }
    }
}
