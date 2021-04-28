using System;

namespace TN.TNM.DataAccess.Models.Note
{
    public class NoteDocumentEntityModel
    {
        public Guid NoteDocumentId { get; set; }
        public Guid NoteId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentSize { get; set; }
        public string DocumentUrl { get; set; }
        public string Base64Url { get; set; }
        public bool Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
