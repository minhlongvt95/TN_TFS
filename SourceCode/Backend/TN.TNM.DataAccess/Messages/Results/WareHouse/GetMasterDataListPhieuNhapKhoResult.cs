using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class GetMasterDataListPhieuNhapKhoResult : BaseResult
    {
        public List<CategoryEntityModel> ListStatus { get; set; }
        public List<WareHouseEntityModel> ListWarehouse { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<ProductEntityModel> ListProduct { get; set; }
    }
}
