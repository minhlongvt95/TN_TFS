using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Product;
using TN.TNM.BusinessLogic.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class GetMasterDataOrderDetailDialogResponse : BaseResponse
    {
        public List<CategoryModel> ListUnitMoney { get; set; }
        public List<WareHouseModel> ListWareHouse { get; set; }
        public List<CategoryModel> ListUnitProduct { get; set; }
        public List<ProductModel> ListProduct { get; set; }
    }
}
