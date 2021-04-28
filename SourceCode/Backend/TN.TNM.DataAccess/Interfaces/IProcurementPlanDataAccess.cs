using TN.TNM.DataAccess.Messages.Parameters.ProcurementPlan;
using TN.TNM.DataAccess.Messages.Results.ProcurementPlan;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IProcurementPlanDataAccess
    {
        CreateProcurementPlanResult CreateProcurementPlan(CreateProcurementPlanParameter parameter);
        GetAllProcurementPlanResult GetAllProcurementPlan(GetAllProcurementPlanParameter parameter);
        SearchProcurementPlanResult SearchProcurementPlan(SearchProcurementPlanParameter parameter);
        GetProcurementPlanByIdResult GetProcurementPlanById(GetProcurementPlanByIdParameter parameter);
        EditProcurementPlanByIdResult EditProcurementPlanById(EditProcurementPlanByIdParameter parameter);
    }
}
