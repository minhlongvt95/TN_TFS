using System;

namespace TN.TNM.DataAccess.Messages.Parameters.ProcurementPlan
{
    public class GetProcurementPlanByIdParameter : BaseParameter
    {
        public Guid ProcurementPlanId { get; set; }
    }
}
