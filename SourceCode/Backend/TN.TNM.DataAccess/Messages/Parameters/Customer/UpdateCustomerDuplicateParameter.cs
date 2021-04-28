using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class UpdateCustomerDuplicateParameter:BaseParameter
    {
        public List<Databases.Entities.Customer> lstcontactCustomerDuplicate { get; set; }
        public List<Databases.Entities.Contact> lstcontactContactDuplicate { get; set; }
        public List<Databases.Entities.Contact> lstcontactContact_CON_Duplicate { get; set; }
    }
}
