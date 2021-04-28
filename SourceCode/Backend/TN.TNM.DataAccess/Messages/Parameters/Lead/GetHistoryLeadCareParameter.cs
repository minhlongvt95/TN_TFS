using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class GetHistoryLeadCareParameter: BaseParameter
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public Guid LeadId { get; set; }
    }
}
