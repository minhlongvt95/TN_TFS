using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Messages.Results.Employee;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IEmployeeAssessmentDataAccess
    {
        SearchEmployeeAssessmentResult SearchEmployeeAssessment(SearchEmployeeAssessmentParameter parameter);
        GetAllYearToAssessmentResult GetAllYearToAssessment(GetAllYearToAssessmentParameter parameter);
        EditEmployeeAssessmentResult EditEmployeeAssessment(EditEmployeeAssessmentParameter parameter);
    }
}
