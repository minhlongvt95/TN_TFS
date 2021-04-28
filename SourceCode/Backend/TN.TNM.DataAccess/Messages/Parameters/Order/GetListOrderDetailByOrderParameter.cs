using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class GetListOrderDetailByOrderParameter : BaseParameter
    {
        public Guid OrderId { get; set; }
    }
}
