using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Order
{
    public class GetListOrderByLocalPointParameter : BaseParameter
    {
        public Guid LocalPointId { get; set; }
    }
}
