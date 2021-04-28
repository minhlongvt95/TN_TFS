using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Receivable.Vendor
{
    public class GetReceivableVendorDetailParameter : BaseParameter
    {
        public Guid VendorId { get; set; }
        public DateTime? ReceivalbeDateFrom { get; set; }
        public DateTime? ReceivalbeDateTo { get; set; }
    }
}
