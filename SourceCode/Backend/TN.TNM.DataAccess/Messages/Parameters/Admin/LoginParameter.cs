using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin
{
    public class LoginParameter:BaseParameter
    {
        public User User { get; set; }
    }
}
