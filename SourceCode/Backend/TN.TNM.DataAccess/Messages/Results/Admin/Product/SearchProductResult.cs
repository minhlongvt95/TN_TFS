using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class SearchProductResult:BaseResult
    {
        public List<ProductEntityModel> ProductList { get; set; }
    }
}
