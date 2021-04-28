using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.ProductCategory;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class GetMasterDataSearchInStockReportResult : BaseResult
    {
        public List<ProductCategoryEntityModel> ListProductCategory { get; set; }
        public List<WareHouseEntityModel> ListWareHouse { get; set; }
    }
}
