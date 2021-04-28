using System;
using TN.TNM.DataAccess.Messages.Parameters.ProcurementRequest;

namespace TN.TNM.BusinessLogic.Messages.Requests.ProcurementRequest
{
    public class GetProcurementRequestByIdRequest : BaseRequest<GetProcurementRequestByIdParameter>
    {
        public Guid ProcurementRequestId { get; set; }
        public override GetProcurementRequestByIdParameter ToParameter()
        {
            return new GetProcurementRequestByIdParameter()
            {
                ProcurementRequestId = ProcurementRequestId,
                UserId = UserId
            };
        }
    }
}
