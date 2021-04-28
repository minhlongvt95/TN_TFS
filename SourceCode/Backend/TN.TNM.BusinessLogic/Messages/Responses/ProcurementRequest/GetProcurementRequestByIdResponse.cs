using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Document;
using TN.TNM.BusinessLogic.Models.ProcurementRequest;

namespace TN.TNM.BusinessLogic.Messages.Responses.ProcurementRequest
{
    public class GetProcurementRequestByIdResponse : BaseResponse
    {
        public ProcurementRequestModel ProcurementRequest { get; set; }
        public List<ProcurementRequestItemModel> ListProcurementItem { get; set; }
        public bool IsSendingApprove { get; set; }
        public bool IsApprove { get; set; }
        public bool IsReject { get; set; }
        public List<DocumentModel> ListDocument { get; set; }
    }
}
