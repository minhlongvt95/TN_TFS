using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class CreateEmployeeSalaryParameter : BaseParameter
    {
        public EmployeeSalary EmployeeSalary { get; set; }
    }
}
