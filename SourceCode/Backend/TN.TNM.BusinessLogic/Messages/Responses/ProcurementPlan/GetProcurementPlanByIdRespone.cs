using TN.TNM.BusinessLogic.Models.ProcurementPlan;

namespace TN.TNM.BusinessLogic.Messages.Responses.ProcurementPlan
{
    public class GetProcurementPlanByIdRespone : BaseResponse
    {
        public ProcurementPlanModel ProcurementPlan { get; set; }
    }
}
