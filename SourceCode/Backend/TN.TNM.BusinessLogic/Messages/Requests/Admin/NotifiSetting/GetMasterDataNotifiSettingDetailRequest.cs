using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.NotifiSetting;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.NotifiSetting
{
    public class GetMasterDataNotifiSettingDetailRequest : BaseRequest<GetMasterDataNotifiSettingDetailParameter>
    {
        public Guid NotifiSettingId { get; set; }
        public override GetMasterDataNotifiSettingDetailParameter ToParameter()
        {
            return new GetMasterDataNotifiSettingDetailParameter()
            {
                UserId = UserId,
                NotifiSettingId = NotifiSettingId
            };
        }
    }
}
