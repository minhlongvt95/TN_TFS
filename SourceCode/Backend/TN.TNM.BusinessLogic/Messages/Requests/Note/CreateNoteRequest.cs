using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.DataAccess.Messages.Parameters.Note;

namespace TN.TNM.BusinessLogic.Messages.Requests.Note
{
    public class CreateNoteRequest : BaseRequest<CreateNoteParameter>
    {
        public NoteModel Note { get; set; }
        public Guid? LeadId { get; set; }
        public bool IsUploadFile { get; set; }
        public List<IFormFile> FileList { get; set; }
        public override CreateNoteParameter ToParameter() => new CreateNoteParameter
        {
            Note = Note.ToEntity(),
            LeadId = LeadId,
            UserId = UserId,
            IsUploadFile = IsUploadFile,
            FileList = FileList
        };
    }
}
