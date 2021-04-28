using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.NotifiSetting;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.NotifiSetting
{
    public class ChangeIsEmailRequest : BaseRequest<ChangeIsEmailParameter>
    {
        public Guid NotifiSettingId { get; set; }
        public bool IsEmail { get; set; }

        public override ChangeIsEmailParameter ToParameter()
        {
            return new ChangeIsEmailParameter()
            {
                UserId = UserId,
                NotifiSettingId = NotifiSettingId,
                IsEmail = IsEmail
            };
        }
    }
}
