using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class SearchVendorOrderRequest : BaseRequest<SearchVendorOrderParameter>
    {
        public List<Guid> VendorIdList { get; set; }
        public string VendorModelCode { get; set; }
        public DateTime? VendorOrderDateFrom { get; set; }
        public DateTime? VendorOrderDateTo { get; set; }
        public List<Guid> StatusIdList { get; set; }
        public List<Guid> CreateByIds { get; set; }
        public List<Guid> ListProcurementRequestId { get; set; }
        public List<Guid> ListProductId { get; set; }
        public override SearchVendorOrderParameter ToParameter()
        {
            return new SearchVendorOrderParameter()
            {
                UserId = UserId,
                VendorIdList = VendorIdList,
                VendorModelCode = VendorModelCode,
                VendorOrderDateFrom = VendorOrderDateFrom,
                VendorOrderDateTo = VendorOrderDateTo,
                StatusIdList = StatusIdList,
                CreateyByIds = CreateByIds,
                ListProcurementRequestId = ListProcurementRequestId,
                ListProductId = ListProductId
            };
        }
    }
}
