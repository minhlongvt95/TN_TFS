using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Results.ProcurementRequest
{
    public class GetAllProcurementPlanResult : BaseResult
    {
        public List<Databases.Entities.ProcurementPlan> PRPlanList { get; set; }
    }
}
