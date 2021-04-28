using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.NotifiSetting
{
    public class ChangeIsSmsParameter : BaseParameter
    {
        public Guid NotifiSettingId { get; set; }
        public bool IsSms { get; set; }
    }
}
