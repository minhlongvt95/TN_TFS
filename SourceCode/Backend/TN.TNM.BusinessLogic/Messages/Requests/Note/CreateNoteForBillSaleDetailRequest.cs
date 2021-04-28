using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.DataAccess.Messages.Parameters.Note;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.BusinessLogic.Messages.Requests.Note
{
    public class CreateNoteForBillSaleDetailRequest:BaseRequest<CreateNoteForBillSaleDetailParameter>
    {
        public NoteModel Note { get; set; }
        public List<NoteDocumentModel> ListNoteDocument { get; set; }

        public override CreateNoteForBillSaleDetailParameter ToParameter()
        {
            var listNoteDocument = new List<NoteDocumentEntityModel>();
            if (ListNoteDocument.Count > 0)
            {
                ListNoteDocument.ForEach(item =>
                {
                    var noteDocument = new NoteDocumentEntityModel();
                    noteDocument.NoteDocumentId = item.NoteDocumentId;
                    noteDocument.NoteId = item.NoteId;
                    noteDocument.DocumentName = item.DocumentName;
                    noteDocument.DocumentSize = item.DocumentSize;
                    noteDocument.DocumentUrl = item.DocumentUrl;
                    noteDocument.Active = true;
                    noteDocument.CreatedById = item.CreatedById;
                    noteDocument.CreatedDate = DateTime.Now;
                    noteDocument.UpdatedById = item.UpdatedById;
                    noteDocument.UpdatedDate = item.UpdatedDate;

                    listNoteDocument.Add(noteDocument);
                });
            }

            return new CreateNoteForBillSaleDetailParameter()
            {
                Note = Note.ToEntity(),
                ListNoteDocument = listNoteDocument,
                UserId = UserId
            };
        }
    }
}
