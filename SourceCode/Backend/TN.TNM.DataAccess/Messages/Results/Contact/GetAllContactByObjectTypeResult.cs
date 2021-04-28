using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Contact
{
    public class GetAllContactByObjectTypeResult : BaseResult
    {
        public List<Databases.Entities.Contact> ContactList { get; set; }
    }
}
