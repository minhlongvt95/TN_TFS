using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class GetEmployeeByPersonInChargeRequest : BaseRequest<GetEmployeeByPersonInChargeParameter>
    {
        public Guid EmployeeId { get; set; }
        public override GetEmployeeByPersonInChargeParameter ToParameter()
        {
            return new GetEmployeeByPersonInChargeParameter()
            {
                UserId = UserId,
                EmployeeId = EmployeeId
            };
        }
    }
}
