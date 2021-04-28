using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetAllYearToAssessmentParameter : BaseParameter
    {
        public Guid EmployeeId { get; set; }
    }
}
