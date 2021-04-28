using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class GetDataLeadMeetingByIdParameter : BaseParameter
    {
        public Guid? LeadMeetingId { get; set; }
    }
}
