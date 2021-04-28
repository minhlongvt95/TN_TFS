using System;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Lead
{
    public class GetNoteHistoryRequest : BaseRequest<GetNoteHistoryParameter>
    {
        public Guid? LeadId { get; set; }

        public override GetNoteHistoryParameter ToParameter() => new GetNoteHistoryParameter
        {
            LeadId = LeadId,
            UserId = UserId
        };
    }
}
