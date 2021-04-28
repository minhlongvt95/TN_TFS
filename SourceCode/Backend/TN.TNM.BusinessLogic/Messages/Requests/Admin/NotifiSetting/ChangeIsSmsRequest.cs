using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.NotifiSetting;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.NotifiSetting
{
    public class ChangeIsSmsRequest : BaseRequest<ChangeIsSmsParameter>
    {
        public Guid NotifiSettingId { get; set; }
        public bool IsSms { get; set; }

        public override ChangeIsSmsParameter ToParameter()
        {
            return new ChangeIsSmsParameter()
            {
                UserId = UserId,
                NotifiSettingId = NotifiSettingId,
                IsSms = IsSms
            };
        }
    }
}
