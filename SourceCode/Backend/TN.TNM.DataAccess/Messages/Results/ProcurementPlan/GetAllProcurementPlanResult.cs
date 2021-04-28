using System.Collections.Generic;
using TN.TNM.DataAccess.Models.ProcurementPlan;

namespace TN.TNM.DataAccess.Messages.Results.ProcurementPlan
{
    public class GetAllProcurementPlanResult : BaseResult
    {
        public List<ProcurementPlanEntityModel> ProcurementPlanList { get; set; }
    }
}
