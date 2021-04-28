using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace TN.TNM.DataAccess.Messages.Parameters.Note
{
    public class CreateNoteParameter : BaseParameter
    {
        public Databases.Entities.Note Note { get; set; }
        public Guid? LeadId { get; set; }
        public bool IsUploadFile { get; set; }
        public List<IFormFile> FileList { get; set; }
    }
}

