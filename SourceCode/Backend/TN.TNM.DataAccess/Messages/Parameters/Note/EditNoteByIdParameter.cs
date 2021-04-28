using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace TN.TNM.DataAccess.Messages.Parameters.Note
{
    public class EditNoteByIdParameter : BaseParameter
    {
        public Guid NoteId { get; set; }
        public Guid LeadId  { get; set; }
        public string NoteDescription { get; set; }
        public List<IFormFile> FileList { get; set; }
    }
}
