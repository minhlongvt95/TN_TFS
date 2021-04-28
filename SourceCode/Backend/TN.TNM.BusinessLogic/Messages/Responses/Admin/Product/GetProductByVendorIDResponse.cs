using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Product;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Product
{
    public class GetProductByVendorIDResponse : BaseResponse
    {
        public List<ProductModel> lstProduct { get; set; }
    }
}
