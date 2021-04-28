using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class GetDataAddVendorOrderDetailResult: BaseResult
    {
        public List<CategoryEntityModel> ListMoneyUnit { get; set; }
        public List<Models.Product.ProductEntityModel> ListProductByVendorId { get; set; }
    }
}
