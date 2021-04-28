using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.NotifiSetting;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.NotifiSetting
{
    public class GetMasterDataSearchNotifiSettingRequest : BaseRequest<GetMasterDataSearchNotifiSettingParameter>
    {
        public override GetMasterDataSearchNotifiSettingParameter ToParameter()
        {
            return new GetMasterDataSearchNotifiSettingParameter()
            {
                UserId = UserId
            };
        }
    }
}
