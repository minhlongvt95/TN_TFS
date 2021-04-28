using System;

namespace TN.TNM.DataAccess.Messages.Parameters.WareHouse
{
    public class GetVendorOrderByVendorIdParameter : BaseParameter
    {
        public Guid VendorId { get; set; }
    }
}
