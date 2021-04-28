using System;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class SearchEmployeeInsuranceRequest : BaseRequest<SearchEmployeeInsuranceParameter>
    {
        public Guid? EmployeeId { get; set; }
        public Guid? EmployeeInsuranceId { get; set; }
        public override SearchEmployeeInsuranceParameter ToParameter()
        {
            return new SearchEmployeeInsuranceParameter()
            {
                EmployeeInsuranceId = EmployeeInsuranceId,
                EmployeeId = EmployeeId,
                UserId = UserId
            };
        }
    }
}
