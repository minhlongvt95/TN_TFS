using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.NotifiSetting;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.NotifiSetting
{
    public class ChangeActiveRequest : BaseRequest<ChangeActiveParameter>
    {
        public Guid NotifiSettingId { get; set; }
        public bool Active { get; set; }

        public override ChangeActiveParameter ToParameter()
        {
            return new ChangeActiveParameter()
            {
                UserId = UserId,
                NotifiSettingId = NotifiSettingId,
                Active = Active
            };
        }
    }
}
