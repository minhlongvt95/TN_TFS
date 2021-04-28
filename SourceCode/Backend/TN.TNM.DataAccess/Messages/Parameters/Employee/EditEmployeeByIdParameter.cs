namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class EditEmployeeByIdParameter : BaseParameter
    {
        public Databases.Entities.Employee Employee { get; set; }
        public Databases.Entities.Contact Contact { get; set; }
        public Databases.Entities.User User { get; set; }
        public bool IsResetPass { get; set; }
    }
}
