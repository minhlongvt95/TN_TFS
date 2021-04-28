using System;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetEmployeeSalaryStatusRequest : BaseRequest<GetEmployeeSalaryStatusParameter>
    {
        public Guid CommonId { get; set; }
        public override GetEmployeeSalaryStatusParameter ToParameter()
        {
            return new GetEmployeeSalaryStatusParameter() {
                CommonId = CommonId,
                UserId = UserId
            };
        }
    }
}
