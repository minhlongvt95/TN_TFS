using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class CreateEmployeeParameter : BaseParameter
    {
        public Databases.Entities.Employee Employee { get; set; }
        public Databases.Entities.Contact Contact { get; set; }
        public User User { get; set; }
        public bool IsAccessable { get; set; }
    }
}
