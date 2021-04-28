using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Messages.Results.ProcurementRequest
{
    public class GetDataEditProcurementRequestResult: BaseResult
    {
        public bool IsWorkFlowInActive { get; set; }
        public List<Models.Employee.EmployeeEntityModel> ListApproverEmployeeId { get; set; }
        public Models.ProcurementRequest.ProcurementRequestEntityModel ProcurementRequestEntityModel { get; set; }
        public List<Models.ProcurementRequest.ProcurementRequestItemEntityModel> ListProcurementRequestItemEntityModel { get; set; }
        public List<Models.Document.DocumentEntityModel> ListDocumentModel { get; set; }
        public List<CustomerOrder> ListOrder { get; set; }
        public List<CustomerOrderDetailEntityModel> ListOrderDetail { get; set; }
        public List<NoteEntityModel> ListNote { get; set; }
        public List<string> ListEmailSendTo { get; set; } //List email của người phê duyệt
    }
}
