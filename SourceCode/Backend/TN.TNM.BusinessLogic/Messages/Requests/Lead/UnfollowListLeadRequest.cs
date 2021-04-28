using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class UnfollowListLeadRequest: BaseRequest<UnfollowListLeadParamerter>
    {
        public List<Guid> ListLeadId { get; set; }
        public override UnfollowListLeadParamerter ToParameter()
        {
            return new UnfollowListLeadParamerter()
            {
                UserId = UserId,
                ListLeadId = ListLeadId
            };
        }
    }
}
