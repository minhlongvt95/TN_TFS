using System;
using TN.TNM.DataAccess.Messages.Parameters.Note;

namespace TN.TNM.BusinessLogic.Messages.Requests.Note
{
    public class DisableNoteRequest : BaseRequest<DisableNoteParameter>
    {
        public Guid? NoteId { get; set; }
        public override DisableNoteParameter ToParameter() => new DisableNoteParameter
        {
            NoteId = NoteId,
            UserId = UserId
        };
    }
}
