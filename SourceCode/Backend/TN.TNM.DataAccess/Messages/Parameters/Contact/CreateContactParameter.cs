using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.DataAccess.Messages.Parameters.Contact
{
    public class CreateContactParameter : BaseParameter
    {
        public CustomerOtherContactModel Contact { get; set; }
    }
}
