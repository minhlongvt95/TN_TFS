using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class CloneLeadParameter : BaseParameter
    {
        public Guid LeadId { get; set; }
    }
}
