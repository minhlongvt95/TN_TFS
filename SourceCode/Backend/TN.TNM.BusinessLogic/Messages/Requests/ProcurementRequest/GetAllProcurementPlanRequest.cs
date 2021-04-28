using TN.TNM.DataAccess.Messages.Parameters.ProcurementRequest;

namespace TN.TNM.BusinessLogic.Messages.Requests.ProcurementRequest
{
    public class GetAllProcurementPlanRequest : BaseRequest<GetAllProcurementPlanParameter>
    {
        public override GetAllProcurementPlanParameter ToParameter()
        {
            return new GetAllProcurementPlanParameter() {
                UserId = UserId
            };
        }
    }
}
