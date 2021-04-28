using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class LocalPoint
    {
        public Guid LocalPointId { get; set; }
        public string LocalPointCode { get; set; }
        public string LocalPointName { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public int StatusId { get; set; }
        public Guid LocalAddressId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
