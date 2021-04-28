using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Quote;
using TN.TNM.BusinessLogic.Models.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Responses.Quote
{
    public class GetVendorByProductIdResponse : BaseResponse
    {
        public List<VendorModel> ListVendor { get; set; }
        public List<ObjectAttributeNameProductModel> ListObjectAttributeNameProduct { get; set; }
        public List<ObjectAttributeValueProductModel> ListObjectAttributeValueProduct { get; set; }
        public decimal PriceProduct { get; set; }
    }
}
