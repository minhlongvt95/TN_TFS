using N8.ISUZU.BusinessLogic.Models.Vendor;
using N8.ISUZU.DataAccess.Messages.Parameters.Vendor;
using System;
using System.Collections.Generic;
using System.Text;

namespace N8.ISUZU.BusinessLogic.Messages.Requests.Vendor
{
    public class CreateVendorBankRequest : BaseRequest<CreateVendorBankParameter>
    {
        public VendorBankAccountModel VendorBank { get; set; }
        public override CreateVendorBankParameter ToParameter()
        {
            return new CreateVendorBankParameter()
            {
                UserId = UserId,
                VendorBank = VendorBank.ToEntity()
            };
        }
    }
}
