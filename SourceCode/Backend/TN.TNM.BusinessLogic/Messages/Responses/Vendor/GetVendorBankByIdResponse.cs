using System;
using System.Collections.Generic;
using System.Text;
using N8.ISUZU.BusinessLogic.Models.Vendor;

namespace N8.ISUZU.BusinessLogic.Messages.Responses.Vendor
{
    public class GetVendorBankByIdResponse : BaseResponse
    {
        public VendorBankAccountModel VendorBank { get; set; }
    }
}
