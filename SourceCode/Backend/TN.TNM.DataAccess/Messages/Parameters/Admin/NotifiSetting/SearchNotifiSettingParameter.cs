using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.NotifiSetting;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.NotifiSetting
{
    public class SearchNotifiSettingParameter : BaseParameter
    {
        public string NotifiSettingName { get; set; }
        public List<ScreenEntityModel> ListScreen { get; set; }
    }
}
