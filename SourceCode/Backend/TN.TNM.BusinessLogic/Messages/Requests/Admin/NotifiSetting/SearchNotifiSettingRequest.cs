using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.NotifiSetting;
using TN.TNM.DataAccess.Models.NotifiSetting;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.NotifiSetting
{
    public class SearchNotifiSettingRequest : BaseRequest<SearchNotifiSettingParameter>
    {
        public string NotifiSettingName { get; set; }
        public List<ScreenEntityModel> ListScreen { get; set; }

        public override SearchNotifiSettingParameter ToParameter()
        {
            return new SearchNotifiSettingParameter()
            {
                UserId = UserId,
                NotifiSettingName = NotifiSettingName,
                ListScreen = ListScreen
            };
        }
    }
}
