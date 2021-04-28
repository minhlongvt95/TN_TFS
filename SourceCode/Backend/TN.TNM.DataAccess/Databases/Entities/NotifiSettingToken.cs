using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class NotifiSettingToken
    {
        public Guid NotifiSettingTokenId { get; set; }
        public string TokenCode { get; set; }
        public string TokenLabel { get; set; }
        public Guid? ScreenId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
