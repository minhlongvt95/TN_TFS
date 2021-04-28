using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Vendor
{
    public class SearchVendorOrderReportParameter : BaseParameter
    {
        public string VendorOrderCode { get; set; }
        public List<Guid> ListSelectedVendorId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string ProductCode { get; set; }
        public List<Guid> ListSelectedStatusId { get; set; }
        public List<Guid> ListSelectedProcurementRequestId { get; set; }
        public string PurchaseContractName { get; set; }
        public string Description { get; set; }
        public List<Guid> ListSelectedEmployeeId { get; set; }
    }
}
