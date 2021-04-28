using TN.TNM.DataAccess.Messages.Parameters.ProcurementPlan;

namespace TN.TNM.BusinessLogic.Messages.Requests.ProcurementPlan
{
    public class GetAllProcurementPlanRequest : BaseRequest<GetAllProcurementPlanParameter>
    {
        public override GetAllProcurementPlanParameter ToParameter()
        {
            return new GetAllProcurementPlanParameter()
            {

            };
        }
    }
}
