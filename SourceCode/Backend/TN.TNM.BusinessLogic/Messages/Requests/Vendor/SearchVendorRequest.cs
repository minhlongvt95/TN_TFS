using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Vendor
{
    public class SearchVendorRequest : BaseRequest<SearchVendorParameter>
    {
        public string VendorName { get; set; }
        public string VendorCode { get; set; }
        public List<Guid> VendorGroupIdList { get; set; }
        public override SearchVendorParameter ToParameter()
        {
            return new SearchVendorParameter() {
                UserId = UserId,
                VendorCode = VendorCode,
                VendorName = VendorName,
                VendorGroupIdList = VendorGroupIdList
            };
        }
    }
}
