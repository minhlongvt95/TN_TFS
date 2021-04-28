using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.ProductAttributeCategory;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Product
{
    public class GetProductAttributeByProductIDResponse : BaseResponse
    {
        public List<ProductAttributeCategoryModel> lstProductAttributeCategory { get; set; }
    }
}
