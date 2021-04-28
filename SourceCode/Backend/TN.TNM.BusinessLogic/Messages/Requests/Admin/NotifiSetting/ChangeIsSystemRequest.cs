using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.NotifiSetting;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.NotifiSetting
{
    public class ChangeIsSystemRequest : BaseRequest<ChangeIsSystemParameter>
    {
        public Guid NotifiSettingId { get; set; }
        public bool IsSystem { get; set; }

        public override ChangeIsSystemParameter ToParameter()
        {
            return new ChangeIsSystemParameter()
            {
                UserId = UserId,
                NotifiSettingId = NotifiSettingId,
                IsSystem = IsSystem
            };
        }
    }
}
