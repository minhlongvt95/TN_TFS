using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class CreateVendorParameter : BaseParameter
    {
        public Databases.Entities.Vendor Vendor { get; set; }
        public Databases.Entities.Contact Contact { get; set; }
        public List<Databases.Entities.Contact> VendorContactList { get; set; }
    }
}
