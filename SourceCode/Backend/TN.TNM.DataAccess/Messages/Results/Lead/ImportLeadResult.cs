using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class ImportLeadResult:BaseResult
    {
        public List<Databases.Entities.Lead> lstcontactLeadDuplicate { get; set; }
        public List<Databases.Entities.Contact> lstcontactContactDuplicate { get; set; }
        public List<Databases.Entities.Contact> lstcontactContact_CON_Duplicate { get; set; }
        public List<Databases.Entities.Contact> lstcontactCustomerDuplicate { get; set; }
    }
}
