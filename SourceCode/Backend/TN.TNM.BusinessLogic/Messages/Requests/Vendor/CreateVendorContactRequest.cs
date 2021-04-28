using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class CreateVendorContactRequest: BaseRequest<CreateVendorContactParameter>
    {
        public DataAccess.Models.ContactEntityModel VendorContactModel { get; set; }
        public bool IsUpdate { get; set; }
        public override CreateVendorContactParameter ToParameter()
        {
            return new CreateVendorContactParameter()
            {
                VendorContactModel = VendorContactModel,
                IsUpdate = IsUpdate,
                UserId = UserId
            };
        }
    }
}
