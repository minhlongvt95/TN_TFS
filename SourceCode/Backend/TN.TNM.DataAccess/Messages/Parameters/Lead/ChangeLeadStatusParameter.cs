using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class ChangeLeadStatusParameter: BaseParameter
    {
        public Guid LeadId { get; set; }
        public Guid StatusId { get; set; }
    }
}
