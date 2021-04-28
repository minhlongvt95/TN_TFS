using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Note
    {
        public Note()
        {
            NoteDocument = new HashSet<NoteDocument>();
        }

        public Guid NoteId { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public Guid ObjectId { get; set; }
        public string ObjectType { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string NoteTitle { get; set; }
        public Guid? TenantId { get; set; }

        public ICollection<NoteDocument> NoteDocument { get; set; }
    }
}
