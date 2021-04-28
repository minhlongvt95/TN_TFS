using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetAllCustomerContactResult : BaseResult
    {
        public List<Databases.Entities.Contact> ContactList { get; set; }
    }
}
