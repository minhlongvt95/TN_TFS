using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class LocalAddress
    {
        public Guid LocalAddressId { get; set; }
        public string LocalAddressCode { get; set; }
        public string LocalAddressName { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public Guid BranchId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
