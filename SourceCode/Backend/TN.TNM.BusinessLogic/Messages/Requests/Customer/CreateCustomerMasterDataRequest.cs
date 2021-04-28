using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Customer;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class CreateCustomerMasterDataRequest: BaseRequest<CreateCustomerMasterDataParameter>
    {
        public Guid EmployeeId { get; set; }
        public override CreateCustomerMasterDataParameter ToParameter()
        {
            return new CreateCustomerMasterDataParameter
            {
                EmployeeId = EmployeeId
            };
        }
    }
}
