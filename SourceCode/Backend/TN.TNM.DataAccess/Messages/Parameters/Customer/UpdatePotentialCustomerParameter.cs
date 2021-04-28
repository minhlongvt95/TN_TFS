using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class UpdatePotentialCustomerParameter: BaseParameter
    {
        public DataAccess.Databases.Entities.Customer Customer { get; set; }
        public ContactEntityModel Contact { get; set; }
        public List<DataAccess.Models.ContactEntityModel> ListContact { get; set; }
        public List<Guid?> ListDocumentIdNeedRemove { get; set; }
        public List<DataAccess.Models.Document.LinkOfDocumentEntityModel> ListLinkOfDocument { get; set; }
        public List<DataAccess.Models.Customer.PotentialCustomerProductEntityModel> ListCustomerProduct { get; set; }
    }
}
