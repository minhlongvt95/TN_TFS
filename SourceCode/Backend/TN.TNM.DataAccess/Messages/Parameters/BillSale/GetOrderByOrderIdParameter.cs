using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.BillSale
{
    public class GetOrderByOrderIdParameter:BaseParameter
    {
        public Guid OrderId { get; set; }
    }
}
