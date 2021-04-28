using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Product
{
    public class GetProductByVendorIDParameter : BaseParameter
    {
        public Guid VendorId { get; set; }
    }
}
