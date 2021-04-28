using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Order;

namespace TN.TNM.BusinessLogic.Messages.Responses.ProcurementRequest
{
    public class GetDataCreateProcurementRequestResponse: BaseResponse
    {
        public bool IsWorkFlowInActive { get; set; }
        public List<DataAccess.Models.Employee.EmployeeEntityModel> ListApproverEmployeeId { get; set; }
        public DataAccess.Models.Employee.EmployeeEntityModel CurrentEmployeeModel { get; set; }
        public List<CustomerOrderModel> ListOrder { get; set; }
        public List<CustomerOrderDetailModel> ListOrderDetail { get; set; }
    }
}
