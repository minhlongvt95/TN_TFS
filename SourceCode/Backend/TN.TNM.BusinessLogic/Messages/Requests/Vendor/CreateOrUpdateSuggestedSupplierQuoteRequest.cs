using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class CreateOrUpdateSuggestedSupplierQuoteRequest : BaseRequest<CreateOrUpdateSuggestedSupplierQuoteParameter>
    {
        public SuggestedSupplierQuotesEntityModel SuggestedSupplierQuotes { get; set; }
        public List<SuggestedSupplierQuotesDetailEntityModel> ListSuggestedSupplierQuotesDetail { get; set; }

        public override CreateOrUpdateSuggestedSupplierQuoteParameter ToParameter()
        {
            return new CreateOrUpdateSuggestedSupplierQuoteParameter
            {
                ListSuggestedSupplierQuotesDetail = ListSuggestedSupplierQuotesDetail,
                SuggestedSupplierQuotes = SuggestedSupplierQuotes,
                UserId = UserId
            };
        }
    }
}
