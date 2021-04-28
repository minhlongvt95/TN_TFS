using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Product;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Product
{
    public class GetMasterDataPriceProductResponse : BaseResponse
    {
        public List<ProductModel> ListProduct { get; set; }
        public List<CategoryModel> ListCategory { get; set; }
        public List<PriceProductModel> ListPrice { get; set; }
    }
}
