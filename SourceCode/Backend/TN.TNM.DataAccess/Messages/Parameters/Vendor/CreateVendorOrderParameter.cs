using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class CreateVendorOrderParameter : BaseParameter
    {
        public VendorOrder VendorOrder { get; set; }
        public List<VendorOrderDetail> VendorOrderDetail { get; set; }
        public List<VendorOrderProcurementRequestMappingEntityModel> ListVendorOrderProcurementRequestMapping { get; set; }
        public List<VendorOrderCostDetailEntityModel> ListVendorOrderCostDetail { get; set; }
    }
}
