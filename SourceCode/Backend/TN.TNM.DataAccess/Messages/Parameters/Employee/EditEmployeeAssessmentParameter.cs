using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class EditEmployeeAssessmentParameter : BaseParameter
    {
        public List<EmployeeAssessment> ListEmployeeAssessment { get; set; }
    }
}
