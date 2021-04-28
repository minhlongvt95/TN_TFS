using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.ProductCategory;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class GetMasterDataSearchInStockReportResponse : BaseResponse
    {
        public List<ProductCategoryEntityModel> ListProductCategory { get; set; }
        public List<WareHouseEntityModel> ListWareHouse { get; set; }
    }
}
