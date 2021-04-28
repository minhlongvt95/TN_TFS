using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Product
{
    public class GetDataQuickCreateProductResponse: BaseResponse
    {
        public List<string> ListProductCode { get; set; }
        public List<DataAccess.Models.CategoryEntityModel> ListProductUnit { get; set; }
        public List<DataAccess.Models.CategoryEntityModel> ListPriceInventory { get; set; }
        public List<DataAccess.Models.CategoryEntityModel> ListProperty { get; set; }
    }
}
