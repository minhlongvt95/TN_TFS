using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.NotifiSetting;

namespace TN.TNM.DataAccess.Messages.Results.Admin.NotifiSetting
{
    public class GetMasterDataSearchNotifiSettingResult : BaseResult
    {
        public List<ScreenEntityModel> ListScreen { get; set; }
    }
}
