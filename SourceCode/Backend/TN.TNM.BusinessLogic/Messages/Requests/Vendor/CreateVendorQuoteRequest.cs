using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class CreateVendorQuoteRequest : BaseRequest<CreateVendorQuoteParameter>
    {
        public List<ListVendorQuoteParameter> SuggestedSupplierQuoteList { get; set; }
        public override CreateVendorQuoteParameter ToParameter()
        {
            return new CreateVendorQuoteParameter()
            {
                SuggestedSupplierQuoteList = SuggestedSupplierQuoteList,
                UserId = UserId
            };
        }
    }
}
