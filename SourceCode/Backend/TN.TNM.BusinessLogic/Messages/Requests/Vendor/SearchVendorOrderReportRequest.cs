using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class SearchVendorOrderReportRequest : BaseRequest<SearchVendorOrderReportParameter>
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

        public override SearchVendorOrderReportParameter ToParameter()
        {
            return new SearchVendorOrderReportParameter()
            {
                UserId = UserId,
                VendorOrderCode = VendorOrderCode,
                ListSelectedVendorId = ListSelectedVendorId,
                FromDate = FromDate,
                ToDate = ToDate,
                ProductCode = ProductCode,
                ListSelectedStatusId = ListSelectedStatusId,
                ListSelectedProcurementRequestId = ListSelectedProcurementRequestId,
                PurchaseContractName = PurchaseContractName,
                Description = Description,
                ListSelectedEmployeeId = ListSelectedEmployeeId
            };
        }
    }
}
