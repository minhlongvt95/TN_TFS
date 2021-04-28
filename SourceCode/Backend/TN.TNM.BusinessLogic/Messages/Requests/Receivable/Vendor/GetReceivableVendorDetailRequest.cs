using System;
using TN.TNM.DataAccess.Messages.Parameters.Receivable.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Requests.Receivable.Vendor
{
    public class GetReceivableVendorDetailRequest : BaseRequest<GetReceivableVendorDetailParameter>
    {
        public Guid VendorId { get; set; }
        public DateTime? ReceivalbeDateFrom { get; set; }
        public DateTime? ReceivalbeDateTo { get; set; }
        public override GetReceivableVendorDetailParameter ToParameter()
        {
            return new GetReceivableVendorDetailParameter
            {
                VendorId = VendorId,
                ReceivalbeDateTo = ReceivalbeDateTo,
                ReceivalbeDateFrom = ReceivalbeDateFrom
            };
        }
    }
}
