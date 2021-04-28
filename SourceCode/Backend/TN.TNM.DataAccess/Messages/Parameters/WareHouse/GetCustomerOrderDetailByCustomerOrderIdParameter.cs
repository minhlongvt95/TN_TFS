using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.WareHouse
{
    public class GetCustomerOrderDetailByCustomerOrderIdParameter:BaseParameter
    {
        public int TypeWarehouseVocher { get; set; }
        public List<Guid> ListCustomerOrderId { get; set; }

    }
}
