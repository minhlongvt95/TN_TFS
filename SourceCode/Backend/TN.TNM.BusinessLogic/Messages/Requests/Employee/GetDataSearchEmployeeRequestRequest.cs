using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetDataSearchEmployeeRequestRequest : BaseRequest<GetDataSearchEmployeeRequestParameter>
    {
        public override GetDataSearchEmployeeRequestParameter ToParameter()
        {
            return new GetDataSearchEmployeeRequestParameter
            {
                UserId = UserId
            };
        }
    }
}
