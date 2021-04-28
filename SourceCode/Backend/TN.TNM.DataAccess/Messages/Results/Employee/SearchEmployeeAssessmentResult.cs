using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class SearchEmployeeAssessmentResult : BaseResult
    {
        public List<EmployeeAssessment> ListEmployeeAssessment { get; set; }
    }
}
