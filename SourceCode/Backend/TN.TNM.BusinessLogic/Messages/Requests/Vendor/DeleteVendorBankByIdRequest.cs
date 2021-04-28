using System;
using System.Collections.Generic;
using System.Text;
using N8.ISUZU.DataAccess.Messages.Parameters.Vendor;

namespace N8.ISUZU.BusinessLogic.Messages.Requests.Vendor
{
    public class DeleteVendorBankByIdRequest : BaseRequest<DeleteVendorBankByIdParameter>
    {
        public Guid VendorBankId { get; set; }
        public override DeleteVendorBankByIdParameter ToParameter()
        {
            return new DeleteVendorBankByIdParameter()
            {
                UserId = UserId,
                VendorBankId = VendorBankId
            };
        }
    }
}
