using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Results.SaleBidding
{
    public class GetVendorMappingResult : BaseResult
    {
        public List<ProductVendorMappingEntityModel> ListVendor { get; set; }
    }
}
