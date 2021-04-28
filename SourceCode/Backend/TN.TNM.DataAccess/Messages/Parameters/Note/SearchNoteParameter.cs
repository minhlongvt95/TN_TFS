using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Note
{
    public class SearchNoteParameter : BaseParameter
    {
        public string Keyword { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Guid LeadId { get; set; }
    }
}