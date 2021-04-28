using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Product;

namespace TN.TNM.BusinessLogic.Messages.Responses.SaleBidding
{
    public class GetVendorMappingResponse : BaseResponse
    {
        public List<ProductVendorMappingModel> ListVendor { get; set; }
    }
}
