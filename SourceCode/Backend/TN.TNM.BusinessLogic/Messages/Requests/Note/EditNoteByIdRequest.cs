using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TN.TNM.DataAccess.Messages.Parameters.Note;

namespace TN.TNM.BusinessLogic.Messages.Requests.Note
{
    public class EditNoteByIdRequest : BaseRequest<EditNoteByIdParameter>
    {
        public Guid NoteId { get; set; }
        public string NoteDescription { get; set; }
        public List<IFormFile> FileList { get; set; }
        public Guid LeadId { get; set; }
        public override EditNoteByIdParameter ToParameter()
        {
            return new EditNoteByIdParameter()
            {
                NoteId = NoteId,
                NoteDescription = NoteDescription,
                FileList = FileList,
                LeadId = LeadId
            };
        }
    }
}
