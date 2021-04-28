using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class CreateVendorContactParameter: BaseParameter
    {
        public Models.ContactEntityModel VendorContactModel { get; set; }
        public bool IsUpdate { get; set; }
    }
}
