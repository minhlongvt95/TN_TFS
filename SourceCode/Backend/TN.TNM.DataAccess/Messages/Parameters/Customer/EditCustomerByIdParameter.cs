using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class EditCustomerByIdParameter : BaseParameter
    {
        public Databases.Entities.Customer Customer { get; set; }
        public Databases.Entities.Contact Contact { get; set; }
        public List<Databases.Entities.Contact> ContactList { get; set; }
    }
}
