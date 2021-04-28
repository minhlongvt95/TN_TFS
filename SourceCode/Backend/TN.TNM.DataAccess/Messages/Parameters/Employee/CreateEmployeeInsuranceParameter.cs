using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class CreateEmployeeInsuranceParameter : BaseParameter
    {
        public EmployeeInsurance EmployeeInsurance { get; set; }
    }
}
