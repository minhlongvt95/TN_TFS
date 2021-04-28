using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.NotifiSetting;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.NotifiSetting
{
    public class GetMasterDataSearchNotifiSettingResponse : BaseResponse
    {
        public List<ScreenEntityModel> ListScreen { get; set; }
    }
}
