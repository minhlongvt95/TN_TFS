using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetMasterDataOrderDetailDialogResult : BaseResult
    {
        public List<CategoryEntityModel> ListUnitMoney { get; set; }
        public List<WareHouseEntityModel> ListWareHouse { get; set; }
        public List<CategoryEntityModel> ListUnitProduct { get; set; }
        public List<ProductEntityModel> ListProduct { get; set; }
    }
}
