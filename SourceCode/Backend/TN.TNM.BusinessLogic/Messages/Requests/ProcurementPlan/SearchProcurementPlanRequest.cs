using TN.TNM.DataAccess.Messages.Parameters.ProcurementPlan;

namespace TN.TNM.BusinessLogic.Messages.Requests.ProcurementPlan
{
    public class SearchProcurementPlanRequest : BaseRequest<SearchProcurementPlanParameter>

    {
        public string ProcurementPlanYear { get; set; }
        public string ProcurementPlanMonth { get; set; }
        public override SearchProcurementPlanParameter ToParameter() => new SearchProcurementPlanParameter()
        {
            ProcurementPlanYear = ProcurementPlanYear,
            ProcurementPlanMonth = ProcurementPlanMonth
        };
    }
}
