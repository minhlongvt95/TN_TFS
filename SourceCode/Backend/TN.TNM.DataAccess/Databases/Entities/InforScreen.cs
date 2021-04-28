using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class InforScreen
    {
        public Guid InforScreenId { get; set; }
        public string InforScreenName { get; set; }
        public string InforScreenCode { get; set; }
        public Guid? ScreenId { get; set; }
        public Guid? TenantId { get; set; }
        public bool IsDateTime { get; set; }
    }
}
