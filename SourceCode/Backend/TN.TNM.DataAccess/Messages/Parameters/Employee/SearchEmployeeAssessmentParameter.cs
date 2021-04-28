using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class SearchEmployeeAssessmentParameter : BaseParameter
    {
        public Guid EmployeeId { get; set; }
        public int Year { get; set; }
    }
}
