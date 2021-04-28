using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Vendor;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Responses.WareHouse
{
    public class GetVendorOrderByVendorIdResponse : BaseResponse
    {
        public List<VendorOrderEntityModel> ListVendorOrder { get; set; }
    }
}
