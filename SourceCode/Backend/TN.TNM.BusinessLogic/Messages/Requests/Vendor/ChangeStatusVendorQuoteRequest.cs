using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class ChangeStatusVendorQuoteRequest : BaseRequest<ChangeStatusVendorQuoteParameter>
    {
        public Guid SuggestedSupplierQuoteId { get; set; }
        public Guid StatusId { get; set; }
        public override ChangeStatusVendorQuoteParameter ToParameter()
        {
            return new ChangeStatusVendorQuoteParameter
            {
                SuggestedSupplierQuoteId = SuggestedSupplierQuoteId,
                StatusId = StatusId,
                UserId = UserId
            };
        }
    }
}
