using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Results.SaleBidding
{
    public class GetMasterDataSaleBiddingAddEditProductDialogResult:BaseResult
    {
        public List<CategoryEntityModel> ListUnitMoney { get; set; }
        public List<CategoryEntityModel> ListUnitProduct { get; set; }
        public List<VendorEntityModel> ListVendor { get; set; }
        public List<Product> ListProduct { get; set; }
    }
}
