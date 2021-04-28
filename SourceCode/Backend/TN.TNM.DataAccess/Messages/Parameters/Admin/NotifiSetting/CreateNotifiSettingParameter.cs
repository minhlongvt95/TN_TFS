using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.NotifiSetting;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.NotifiSetting
{
    public class CreateNotifiSettingParameter : BaseParameter
    {
        public Databases.Entities.NotifiSetting NotifiSetting { get; set; }
        public List<NotifiSpecialEntityModel> ListNotifiSpecial { get; set; }
    }
}
