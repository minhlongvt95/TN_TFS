namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class EditEmployeeRequestByIdParameter : BaseParameter
    {
        public Databases.Entities.EmployeeRequest EmployeeRequest { get; set; }
    }
}
