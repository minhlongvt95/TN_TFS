using TN.TNM.BusinessLogic.Models.ProcurementPlan;
using TN.TNM.DataAccess.Messages.Parameters.ProcurementPlan;

namespace TN.TNM.BusinessLogic.Messages.Requests.ProcurementPlan

{
    public class CreateProcurementPlanRequest : BaseRequest<CreateProcurementPlanParameter>
    {
        public ProcurementPlanModel ProcurementPlan { get; set; }

        //hàm map giữa model trên DB và model được truyền lên từ html
        public override CreateProcurementPlanParameter ToParameter() => new CreateProcurementPlanParameter()
        {
            ProcurementPlan = ProcurementPlan.ToEntity()
        };
    }
}
