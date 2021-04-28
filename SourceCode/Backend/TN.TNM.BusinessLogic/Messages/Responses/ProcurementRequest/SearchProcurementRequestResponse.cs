using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.ProcurementRequest;

namespace TN.TNM.BusinessLogic.Messages.Responses.ProcurementRequest
{
    public class SearchProcurementRequestResponse : BaseResponse
    {
        public List<ProcurementRequestModel> ListProcurementRequest { get; set; }
    }
}
