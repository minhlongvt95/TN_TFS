using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Product;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.Product
{
    public class SearchProductRequest:BaseRequest<SearchProductParameter>
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public List<Guid> ListProductCategory { get; set; }
        public List<Guid> ListVendor { get; set; }

        public override SearchProductParameter ToParameter() => new SearchProductParameter()
        {
            ProductName = this.ProductName,
            ProductCode = this.ProductCode,
            ListProductCategory = this.ListProductCategory,
            ListVendor=this.ListVendor
        };

    }
}
