using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class GetLocalPointByLocalAddressParameter : BaseParameter
    {
        public Guid LocalAddressId { get; set; }
    }
}
