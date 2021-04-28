using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.NotifiSetting;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.NotifiSetting
{
    public class SearchNotifiSettingResponse : BaseResponse
    {
        public List<NotifiSettingEntityModel> ListNotifiSetting { get; set; }
    }
}
