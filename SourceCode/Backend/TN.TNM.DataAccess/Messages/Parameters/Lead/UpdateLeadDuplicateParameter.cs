using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class UpdateLeadDuplicateParameter : BaseParameter
    {
        public List<Databases.Entities.Lead> lstcontactLeadDuplicate { get; set; }
        public List<Databases.Entities.Contact> lstcontactContactDuplicate { get; set; }
    }

}
