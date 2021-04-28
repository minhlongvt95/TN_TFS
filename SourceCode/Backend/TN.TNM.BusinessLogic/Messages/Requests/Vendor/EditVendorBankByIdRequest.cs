using System;
using System.Collections.Generic;
using System.Text;
using N8.ISUZU.BusinessLogic.Models.Vendor;
using N8.ISUZU.DataAccess.Messages.Parameters.Vendor;

namespace N8.ISUZU.BusinessLogic.Messages.Requests.Vendor
{
    public class EditVendorBankByIdRequest : BaseRequest<EditVendorBankByIdParameter>
    {
        public VendorBankAccountModel VendorBank { get; set; }
        public override EditVendorBankByIdParameter ToParameter()
        {
            return new EditVendorBankByIdParameter()
            {
                UserId = UserId,
                VendorBank = VendorBank.ToEntity()
            };
        }
    }
}
