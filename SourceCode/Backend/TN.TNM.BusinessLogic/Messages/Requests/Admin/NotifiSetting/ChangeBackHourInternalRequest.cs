using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.NotifiSetting;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.NotifiSetting
{
    public class ChangeBackHourInternalRequest : BaseRequest<ChangeBackHourInternalParameter>
    {
        public Guid NotifiSettingId { get; set; }
        public int BackHourInternal { get; set; }

        public override ChangeBackHourInternalParameter ToParameter()
        {
            return new ChangeBackHourInternalParameter()
            {
                UserId = UserId,
                NotifiSettingId = NotifiSettingId,
                BackHourInternal = BackHourInternal
            };
        }
    }
}
