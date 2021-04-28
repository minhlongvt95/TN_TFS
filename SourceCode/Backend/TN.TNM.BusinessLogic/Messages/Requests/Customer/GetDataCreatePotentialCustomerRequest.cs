using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class GetDataCreatePotentialCustomerRequest : BaseRequest<GetDataCreatePotentialCustomerParameter>
    {
        public Guid EmployeeId { get; set; }
        public override GetDataCreatePotentialCustomerParameter ToParameter()
        {
            return new GetDataCreatePotentialCustomerParameter
            {
                UserId = UserId,
                EmployeeId = EmployeeId
            };
        }
    }
}
