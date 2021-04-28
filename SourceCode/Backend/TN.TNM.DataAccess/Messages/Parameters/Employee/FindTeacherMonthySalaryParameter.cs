using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class FindTeacherMonthySalaryParameter:BaseParameter
    {
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public Guid? EmployeePostionId { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

    }
}
