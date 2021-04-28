using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class SearchEmployeeAssessmentResponse : BaseResponse
    {
        public List<EmployeeAssessmentModel> ListEmployeeAssessment { get; set; }
    }
}
