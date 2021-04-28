using System;
using TN.TNM.DataAccess.Messages.Parameters.SaleBidding;

namespace TN.TNM.BusinessLogic.Messages.Requests.SaleBidding
{
    public class GetCustomerByEmployeeIdRequest : BaseRequest<GetCustomerByEmployeeIdParameter>
    {
        public Guid EmployeeId { get; set; }
        public override GetCustomerByEmployeeIdParameter ToParameter()
        {
            return new GetCustomerByEmployeeIdParameter()
            {
                EmployeeId = EmployeeId,
                UserId = UserId
            };
        }
    }
}
