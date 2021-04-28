using TN.TNM.BusinessLogic.Models.ProcurementPlan;
using TN.TNM.DataAccess.Messages.Parameters.ProcurementPlan;

namespace TN.TNM.BusinessLogic.Messages.Requests.ProcurementPlan
{
    public class EditProcurementPlanByIdRequest : BaseRequest<EditProcurementPlanByIdParameter>
    {
        public ProcurementPlanModel ProcurementPlan { get; set; }
        public override EditProcurementPlanByIdParameter ToParameter() => new EditProcurementPlanByIdParameter()
        {
            ProcurementPlan = ProcurementPlan.ToEntity(),
            UserId = UserId 
        };
        
    }
}
