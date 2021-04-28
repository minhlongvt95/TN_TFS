using TN.TNM.BusinessLogic.Messages.Requests.Employee;
using TN.TNM.BusinessLogic.Messages.Responses.Employee;

namespace TN.TNM.BusinessLogic.Interfaces.Employee
{
    public interface IEmployeeAssessment
    {
        SearchEmployeeAssessmentResponse SearchEmployeeAssessment(SearchEmployeeAssessmentRequest request);
        GetAllYearToAssessmentResponse GetAllYearToAssessment(GetAllYearToAssessmentRequest request);
        EditEmployeeAssessmentResponse EditEmployeeAssessment(EditEmployeeAssessmentRequest request);
    }
}
