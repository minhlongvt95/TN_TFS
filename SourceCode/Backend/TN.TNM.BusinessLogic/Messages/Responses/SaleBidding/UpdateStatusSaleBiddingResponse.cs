using TN.TNM.BusinessLogic.Models.Note;

namespace TN.TNM.BusinessLogic.Messages.Responses.SaleBidding
{
    public class UpdateStatusSaleBiddingResponse : BaseResponse
    {
        public NoteModel Note { get; set; }
    }
}
