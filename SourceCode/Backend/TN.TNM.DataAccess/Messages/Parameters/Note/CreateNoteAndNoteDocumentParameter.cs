using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace TN.TNM.DataAccess.Messages.Parameters.Note
{
    public class CreateNoteAndNoteDocumentParameter : BaseParameter
    {
        public Guid LeadId { get; set; }
        public List<IFormFile> FileList { get; set; }
    }
}
