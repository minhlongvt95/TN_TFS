using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Vendor;
using Entities = TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class GetVendorOrderByVendorIdResult : BaseResult
    {
        public List<VendorOrderEntityModel> ListVendorOrder { get; set; }
    }
}
