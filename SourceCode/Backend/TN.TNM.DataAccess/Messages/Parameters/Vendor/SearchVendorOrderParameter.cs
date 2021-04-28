using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class SearchVendorOrderParameter : BaseParameter
    {
        public List<Guid> VendorIdList { get; set; }
        public string VendorModelCode { get; set; }
        public DateTime? VendorOrderDateFrom { get; set; }
        public DateTime? VendorOrderDateTo { get; set; }
        public List<Guid> StatusIdList { get; set; }
        public List<Guid> CreateyByIds { get; set; }
        public List<Guid> ListProcurementRequestId { get; set; }
        public List<Guid> ListProductId { get; set; }
    }
}
