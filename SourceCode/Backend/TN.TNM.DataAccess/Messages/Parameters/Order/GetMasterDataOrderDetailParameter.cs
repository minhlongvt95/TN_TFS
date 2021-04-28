using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class GetMasterDataOrderDetailParameter : BaseParameter
    {
        public Guid OrderId { get; set; }
    }
}
