using System.Collections.Generic;
using TN.TNM.DataAccess.Models.ProcurementPlan;

namespace TN.TNM.DataAccess.Messages.Results.ProcurementPlan
{
    public class SearchProcurementPlanResult
    {
        public List<ProcurementPlanEntityModel> ProcurementPlanList { get; set; }
    }
}
