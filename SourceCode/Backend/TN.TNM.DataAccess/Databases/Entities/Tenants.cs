using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Tenants
    {
        public Guid TenantId { get; set; }
        public string TenantName { get; set; }
        public string TenantHost { get; set; }
        public string TenantMode { get; set; }
        public int? UserNumber { get; set; }
        public string Status { get; set; }
    }
}
