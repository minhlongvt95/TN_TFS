using System;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class SearchEmployeeAssessmentRequest : BaseRequest<SearchEmployeeAssessmentParameter>
    {
        public Guid EmployeeId { get; set; }
        public int Year { get; set; }
        public override SearchEmployeeAssessmentParameter ToParameter()
        {
            return new SearchEmployeeAssessmentParameter()
            {
                EmployeeId = EmployeeId,
                Year = Year,
                UserId = UserId
            };
        }
    }
}
