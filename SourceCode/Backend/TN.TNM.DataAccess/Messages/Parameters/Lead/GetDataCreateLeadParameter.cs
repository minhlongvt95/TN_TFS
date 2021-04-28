using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class GetDataCreateLeadParameter: BaseParameter
    {
        public Guid CustomerId { get; set; }
    }
}
