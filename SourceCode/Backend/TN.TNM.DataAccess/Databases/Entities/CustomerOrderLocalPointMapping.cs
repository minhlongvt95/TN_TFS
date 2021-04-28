using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CustomerOrderLocalPointMapping
    {
        public Guid CustomerOrderLocalPointMappingId { get; set; }
        public Guid? OrderId { get; set; }
        public Guid? LocalPointId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
