using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.NotifiSetting;
using TN.TNM.DataAccess.Messages.Parameters.Admin.NotifiSetting;
using TN.TNM.DataAccess.Models.NotifiSetting;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.NotifiSetting
{
    public class UpdateNotifiSettingRequest : BaseRequest<UpdateNotifiSettingParameter>
    {
        public NotifiSettingModel NotifiSetting { get; set; }
        public List<NotifiSpecialEntityModel> ListNotifiSpecial { get; set; }

        public override UpdateNotifiSettingParameter ToParameter()
        {
            return new UpdateNotifiSettingParameter()
            {
                UserId = UserId,
                NotifiSetting = NotifiSetting.ToEntity(),
                ListNotifiSpecial = ListNotifiSpecial
            };
        }
    }
}
