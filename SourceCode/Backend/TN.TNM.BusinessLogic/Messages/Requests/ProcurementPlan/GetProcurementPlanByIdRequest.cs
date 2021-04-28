using System;
using TN.TNM.DataAccess.Messages.Parameters.ProcurementPlan;

namespace TN.TNM.BusinessLogic.Messages.Requests.ProcurementPlan
{
    public class GetProcurementPlanByIdRequest : BaseRequest<GetProcurementPlanByIdParameter>
    {
        public Guid ProcurementPlanId { get; set; }
        public override GetProcurementPlanByIdParameter ToParameter()
        {
            return new GetProcurementPlanByIdParameter()
            {
                ProcurementPlanId = ProcurementPlanId
            };
        }
    }
}
