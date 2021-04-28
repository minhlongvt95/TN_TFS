using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.NotifiSetting;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.NotifiSetting
{
    public class ChangeSendInternalRequest : BaseRequest<ChangeSendInternalParameter>
    {
        public Guid NotifiSettingId { get; set; }
        public bool SendInternal { get; set; }

        public override ChangeSendInternalParameter ToParameter()
        {
            return new ChangeSendInternalParameter()
            {
                UserId = UserId,
                NotifiSettingId = NotifiSettingId,
                SendInternal = SendInternal
            };
        }
    }
}
