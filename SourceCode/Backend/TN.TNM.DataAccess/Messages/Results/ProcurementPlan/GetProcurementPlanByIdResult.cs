using TN.TNM.DataAccess.Models.ProcurementPlan;

namespace TN.TNM.DataAccess.Messages.Results.ProcurementPlan
{
    public class GetProcurementPlanByIdResult : BaseResult 
    {
        public ProcurementPlanEntityModel ProcurementPlan { get; set; }

    }
}
