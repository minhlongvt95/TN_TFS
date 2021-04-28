using System;
using TN.TNM.DataAccess.Messages.Parameters.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class GetAllYearToAssessmentRequest : BaseRequest<GetAllYearToAssessmentParameter>
    {
        public Guid EmployeeId { get; set; }
        public override GetAllYearToAssessmentParameter ToParameter()
        {
            return new GetAllYearToAssessmentParameter()
            {
                EmployeeId = EmployeeId,
                UserId = UserId
            };
        }
    }
}
