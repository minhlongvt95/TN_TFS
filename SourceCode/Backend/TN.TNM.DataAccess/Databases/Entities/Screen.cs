using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Screen
    {
        public Guid ScreenId { get; set; }
        public string ScreenName { get; set; }
        public string ScreenCode { get; set; }
        public Guid? TenantId { get; set; }
    }
}
