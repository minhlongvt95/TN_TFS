using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class GetMasterDataCreateSuggestedSupplierQuoteRequest : BaseRequest<GetMasterDataCreateSuggestedSupplierQuoteParameter>
    {
        public Guid SuggestedSupplierQuoteId { get; set; }
        public override GetMasterDataCreateSuggestedSupplierQuoteParameter ToParameter()
        {
            return new GetMasterDataCreateSuggestedSupplierQuoteParameter
            {
                UserId = UserId,
                SuggestedSupplierQuoteId = SuggestedSupplierQuoteId
            };
        }
    }
}
