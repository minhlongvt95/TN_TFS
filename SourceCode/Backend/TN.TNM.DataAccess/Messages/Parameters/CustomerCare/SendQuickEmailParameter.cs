using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.CustomerCare
{
    public class SendQuickEmailParameter : BaseParameter
    {
        public Queue Queue { get; set; }
    }
}
