using TN.TNM.BusinessLogic.Messages.Requests.ProcurementPlan;
using TN.TNM.BusinessLogic.Messages.Responses.ProcurementPlan;

namespace TN.TNM.BusinessLogic.Interfaces.ProcurementPlan
{
    public interface IProcurementPlan
    {
        CreateProcurementPlanResponse CreateProcurementPlan(CreateProcurementPlanRequest request);
        GetAllProcurementPlanResponse GetAllProcurementPlan(GetAllProcurementPlanRequest request);
        SearchProcurementPlanResponse SearchProcurementPlan(SearchProcurementPlanRequest request);
        GetProcurementPlanByIdRespone GetProcurementPlanById(GetProcurementPlanByIdRequest request);
        EditProcurementPlanByIdResponse EditProcurementPlanById(EditProcurementPlanByIdRequest request);
    }
}
