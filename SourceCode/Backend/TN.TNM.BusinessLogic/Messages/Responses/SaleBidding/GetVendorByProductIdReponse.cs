using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Quote;
using TN.TNM.BusinessLogic.Models.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Responses.SaleBidding
{
    public class GetVendorByProductIdReponse : BaseResponse
    {
        public List<VendorModel> ListVendor { get; set; }
        public List<ObjectAttributeNameProductModel> ListObjectAttributeNameProduct { get; set; }
        public List<ObjectAttributeValueProductModel> ListObjectAttributeValueProduct { get; set; }
    }
}
