using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class GetVendorByProductIdParameter : BaseParameter
    {
        public Guid ProductId { get; set; }
        public Guid? CustomerGroupId { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
