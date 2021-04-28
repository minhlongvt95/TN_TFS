using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class EditPersonInChargeRequest : BaseRequest<EditPersonInChargeParameter>
    {
        public List<Guid> ListLeadId { get; set; }
        public Guid EmployeeId { get; set; }
        public override EditPersonInChargeParameter ToParameter()
        {
            return new EditPersonInChargeParameter()
            {
                UserId = UserId,
                ListLeadId = ListLeadId,
                EmployeeId = EmployeeId
            };
        }
    }
}
