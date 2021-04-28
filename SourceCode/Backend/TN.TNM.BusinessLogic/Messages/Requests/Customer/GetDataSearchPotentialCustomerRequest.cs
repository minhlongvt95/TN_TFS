using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class GetDataSearchPotentialCustomerRequest : BaseRequest<GetDataSearchPotentialCustomerParameter>
    {
        public Guid EmployeeId { get; set; }
        public override GetDataSearchPotentialCustomerParameter ToParameter()
        {
            return new GetDataSearchPotentialCustomerParameter
            {
                EmployeeId = EmployeeId
            };
        }
    }
}
