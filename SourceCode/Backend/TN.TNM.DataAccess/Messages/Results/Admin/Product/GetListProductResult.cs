using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class GetListProductResult: BaseResult
    {
        public List<DataAccess.Databases.Entities.ProductCategory> ListProductCategory { get; set; }
        public List<DataAccess.Databases.Entities.Vendor> ListVendor { get; set; }
        public List<DataAccess.Databases.Entities.Category> ListUnit { get; set; }
        public List<DataAccess.Databases.Entities.Category> ListProperty { get; set; }
        public List<DataAccess.Databases.Entities.Category> ListPriceInventory { get; set; }
    }
}
