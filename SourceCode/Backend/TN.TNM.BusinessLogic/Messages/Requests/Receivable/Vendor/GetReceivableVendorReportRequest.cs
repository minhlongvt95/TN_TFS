using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.Receivable.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Receivable.Vendor
{
    public class GetReceivableVendorReportRequest : BaseRequest<GetReceivableVendorReportParameter>
    {
        //public string VendorCode { get; set; }
        public List<Guid> VendorCode { get; set; }
        public string VendorName { get; set; }
        public DateTime? ReceivalbeDateTo { get; set; }
        public override GetReceivableVendorReportParameter ToParameter()
        {
            return new GetReceivableVendorReportParameter
            {
                VendorName = VendorName,
                VendorCode = VendorCode,
                ReceivalbeDateTo = ReceivalbeDateTo
            };
        }
    }
}
