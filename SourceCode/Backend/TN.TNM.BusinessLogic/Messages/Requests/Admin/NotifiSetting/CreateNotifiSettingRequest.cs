using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.NotifiSetting;
using TN.TNM.DataAccess.Messages.Parameters.Admin.NotifiSetting;
using TN.TNM.DataAccess.Models.NotifiSetting;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.NotifiSetting
{
    public class CreateNotifiSettingRequest : BaseRequest<CreateNotifiSettingParameter>
    {
        public NotifiSettingModel NotifiSetting { get; set; }
        public List<NotifiSpecialEntityModel> ListNotifiSpecial { get; set; }

        public override CreateNotifiSettingParameter ToParameter()
        {
            return new CreateNotifiSettingParameter()
            {
                UserId = UserId,
                NotifiSetting = NotifiSetting.ToEntity(),
                ListNotifiSpecial = ListNotifiSpecial
            };
        }
    }
}
