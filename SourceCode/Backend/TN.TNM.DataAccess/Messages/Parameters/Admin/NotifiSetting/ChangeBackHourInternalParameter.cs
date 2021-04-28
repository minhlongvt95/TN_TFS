using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.NotifiSetting
{
    public class ChangeBackHourInternalParameter : BaseParameter
    {
        public Guid NotifiSettingId { get; set; }
        public int BackHourInternal { get; set; }
    }
}
