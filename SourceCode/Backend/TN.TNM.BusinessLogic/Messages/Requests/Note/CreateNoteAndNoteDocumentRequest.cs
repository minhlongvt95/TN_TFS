using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TN.TNM.DataAccess.Messages.Parameters.Note;

namespace TN.TNM.BusinessLogic.Messages.Requests.Note
{
    public class CreateNoteAndNoteDocumentRequest : BaseRequest<CreateNoteAndNoteDocumentParameter>
    {
        public Guid LeadId { get; set; }
        public List<IFormFile> FileList { get; set; }

        public override CreateNoteAndNoteDocumentParameter ToParameter()
        {
            return new CreateNoteAndNoteDocumentParameter()
            {
                LeadId = LeadId,
                FileList = FileList,
                UserId = UserId
            };
        }
    }
}
