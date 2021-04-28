using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.NotifiSetting;

namespace TN.TNM.DataAccess.Messages.Results.Admin.NotifiSetting
{
    public class SearchNotifiSettingResult : BaseResult
    {
        public List<NotifiSettingEntityModel> ListNotifiSetting { get; set; }
    }
}
