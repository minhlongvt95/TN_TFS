using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.WareHouse
{
    public class GetVendorOrderDetailByVenderOrderIdParameter : BaseParameter
    {
        public int TypeWarehouseVocher { get; set; }
        public List<Guid> ListVendorOrderId { get; set; }
    }
}
