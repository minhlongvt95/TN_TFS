using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Email
{
    public class SendEmailVendorOrderParameter : BaseParameter
    {
        public Guid VendorOrderId { get; set; }
    }
}
