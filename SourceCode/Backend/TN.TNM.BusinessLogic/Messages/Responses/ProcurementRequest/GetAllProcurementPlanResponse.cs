using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.ProcurementRequest;

namespace TN.TNM.BusinessLogic.Messages.Responses.ProcurementRequest
{
    public class GetAllProcurementPlanResponse : BaseResponse
    {
        public List<ProcurementPlanModel> PRPlanList { get; set; }
    }
}
