using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Contract;

namespace TN.TNM.BusinessLogic.Messages.Requests.Contract
{
    public class GetListMainContractRequests : BaseRequest<GetListMainContractParameter>
    {
        public Guid EmployeeId { get; set; }

        public override GetListMainContractParameter ToParameter()
        {
            return new GetListMainContractParameter
            {
                EmployeeId = EmployeeId
            };
        }
    }
}
