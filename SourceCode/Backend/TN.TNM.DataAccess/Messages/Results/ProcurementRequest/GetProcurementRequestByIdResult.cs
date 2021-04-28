using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Document;
using TN.TNM.DataAccess.Models.ProcurementRequest;

namespace TN.TNM.DataAccess.Messages.Results.ProcurementRequest
{
    public class GetProcurementRequestByIdResult : BaseResult
    {
        public ProcurementRequestEntityModel ProcurementRequest { get; set; }
        public List<ProcurementRequestItemEntityModel> ListProcurementItem { get; set; }
        public bool IsSendingApprove { get; set; }
        public bool IsApprove { get; set; }
        public bool IsReject { get; set; }
        public List<DocumentEntityModel> listDocument { get; set; }
    }
}
