using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Quote;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetVendorByProductIdResult : BaseResult
    {
        public decimal PriceProduct { get; set; }
        public List<VendorEntityModel> ListVendor { get; set; }
        public List<ObjectAttributeNameProductModel> ListObjectAttributeNameProduct { get; set; }
        public List<ObjectAttributeValueProductModel> ListObjectAttributeValueProduct { get; set; }
    }
}
