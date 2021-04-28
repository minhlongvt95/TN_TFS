using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class GetMasterdataCreateProductResult: BaseResult
    {
        public List<DataAccess.Databases.Entities.Vendor> ListVendor { get; set; }
        public List<DataAccess.Databases.Entities.Category> ListProductMoneyUnit { get; set; }
        public List<DataAccess.Databases.Entities.Category> ListProperty { get; set; }
        public List<DataAccess.Databases.Entities.Category> ListPriceInventory { get; set; }
        public List<DataAccess.Databases.Entities.Category> ListProductUnit { get; set; }
        public List<DataAccess.Databases.Entities.Warehouse> ListWarehouse { get; set; }
        public List<string> ListProductCode { get; set; }
        public List<string> ListProductUnitName { get; set; }
    }
}
