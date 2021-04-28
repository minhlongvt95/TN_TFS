using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class NotifiAction
    {
        public Guid NotifiActionId { get; set; }
        public string NotifiActionName { get; set; }
        public string NotifiActionCode { get; set; }
        public Guid? ScreenId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
