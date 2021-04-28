using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class FindAssistantMonthySalaryParameter:BaseParameter
    {
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeUnit { get; set; }
        public string EmployeeBranch { get; set; }
        public Guid? EmployeePostionId { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

    }
}
