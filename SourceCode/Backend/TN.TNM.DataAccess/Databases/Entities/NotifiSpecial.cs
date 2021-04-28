using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class NotifiSpecial
    {
        public Guid NotifiSpecialId { get; set; }
        public Guid? NotifiSettingId { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
