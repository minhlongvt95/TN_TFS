using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.NotifiSetting
{
    public class ChangeSendInternalParameter : BaseParameter
    {
        public Guid NotifiSettingId { get; set; }
        public bool SendInternal { get; set; }
    }
}
