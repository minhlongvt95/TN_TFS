using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.NotifiSetting
{
    public class NotifiSpecialEntityModel
    {
        public Guid NotifiSpecialId { get; set; }
        public Guid? NotifiSettingId { get; set; }
        public Guid? EmployeeId { get; set; }
    }
}
