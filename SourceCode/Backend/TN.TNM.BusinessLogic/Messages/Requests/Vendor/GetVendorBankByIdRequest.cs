using System;
using System.Collections.Generic;
using System.Text;
using N8.ISUZU.DataAccess.Messages.Parameters.Vendor;

namespace N8.ISUZU.BusinessLogic.Messages.Requests.Vendor
{
    public class GetVendorBankByIdRequest : BaseRequest<GetVendorBankByIdParameter>
    {
        public Guid VendorBankId { get; set; }
        public override GetVendorBankByIdParameter ToParameter()
        {
            return new GetVendorBankByIdParameter()
            {
                UserId = UserId,
                VendorBankId = VendorBankId
            };
        }
    }
}
