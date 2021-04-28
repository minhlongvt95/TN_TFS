using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class GetDataQuickCreateProductResult: BaseResult
    {
        public List<string> ListProductCode { get; set; }
        public List<DataAccess.Models.CategoryEntityModel> ListProductUnit { get; set; }
        public List<DataAccess.Models.CategoryEntityModel> ListPriceInventory { get; set; }
        public List<DataAccess.Models.CategoryEntityModel> ListProperty { get; set; }
    }
}
