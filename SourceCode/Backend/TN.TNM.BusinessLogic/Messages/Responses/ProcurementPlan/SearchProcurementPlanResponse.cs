using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.ProcurementPlan;

namespace TN.TNM.BusinessLogic.Messages.Responses.ProcurementPlan
{
    public class SearchProcurementPlanResponse : BaseResponse
    {
        public List<ProcurementPlanModel> ProcurementPlanList { get; set; }
    }
}
