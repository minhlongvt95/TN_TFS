using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.NotifiSetting;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.NotifiSetting
{
    public class GetMasterDataNotifiSettingCreateRequest : BaseRequest<GetMasterDataNotifiSettingCreateParameter>
    {
        public override GetMasterDataNotifiSettingCreateParameter ToParameter()
        {
            return new GetMasterDataNotifiSettingCreateParameter()
            {
                UserId = UserId
            };
        }
    }
}
