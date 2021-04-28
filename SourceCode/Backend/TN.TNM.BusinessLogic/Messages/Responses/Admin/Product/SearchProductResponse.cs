using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Product
{
    public class SearchProductResponse:BaseResponse
    {
        public List<ProductEntityModel> ProductList { get; set; }
    }
}
