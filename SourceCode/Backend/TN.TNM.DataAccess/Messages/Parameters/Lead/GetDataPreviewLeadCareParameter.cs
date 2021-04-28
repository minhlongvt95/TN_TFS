using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class GetDataPreviewLeadCareParameter : BaseParameter
    {
        public string Mode { get; set; }
        public Guid LeadId { get; set; }
        public Guid LeadCareId { get; set; }
    }
}
