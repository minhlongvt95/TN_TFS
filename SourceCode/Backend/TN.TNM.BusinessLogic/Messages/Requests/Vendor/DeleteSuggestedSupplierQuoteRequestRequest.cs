using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class DeleteSuggestedSupplierQuoteRequestRequest : BaseRequest<DeleteSugestedSupplierQuoteRequestParameter>
    {
        public Guid SuggestedSupplierQuoteId { get; set; }
        public override DeleteSugestedSupplierQuoteRequestParameter ToParameter()
        {
            return new DeleteSugestedSupplierQuoteRequestParameter
            {
                UserId = UserId,
                SuggestedSupplierQuoteId = SuggestedSupplierQuoteId
            };
        }
    }
}
