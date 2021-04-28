using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Note;
using TN.TNM.BusinessLogic.Models.Order;

namespace TN.TNM.BusinessLogic.Messages.Responses.ProcurementRequest
{
    public class GetDataEditProcurementRequestResponse : BaseResponse
    {
        public bool IsWorkFlowInActive { get; set; }
        public List<Models.Employee.EmployeeModel> ListApproverEmployeeId { get; set; }
        public Models.ProcurementRequest.ProcurementRequestModel ProcurementRequestEntityModel { get; set; }
        public List<Models.ProcurementRequest.ProcurementRequestItemModel> ListProcurementRequestItemEntityModel { get; set; }
        public List<DataAccess.Models.Document.DocumentEntityModel> ListDocumentModel { get; set; }
        public List<CustomerOrderModel> ListOrder { get; set; }
        public List<CustomerOrderDetailModel> ListOrderDetail { get; set; }
        public List<NoteModel> ListNote { get; set; }
        public List<string> ListEmailSendTo { get; set; } //List email của người phê duyệt
    }
}
