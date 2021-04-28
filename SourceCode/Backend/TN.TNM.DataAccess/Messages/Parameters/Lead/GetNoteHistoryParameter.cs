using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class GetNoteHistoryParameter : BaseParameter
    {
        public Guid? LeadId { get; set; }
    }
}
