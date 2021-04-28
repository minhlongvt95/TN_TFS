using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Quote
{
    public class DeleteCostParameter : BaseParameter
    {
        public Guid CostId { get; set; }
    }
}
