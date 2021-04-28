using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Order;

namespace TN.TNM.DataAccess.Messages.Results.ProcurementRequest
{
    public class GetDataCreateProcurementRequestResult: BaseResult
    {
        public bool IsWorkFlowInActive { get; set; }
        public List<Models.Employee.EmployeeEntityModel> ListApproverEmployeeId { get; set; }
        public Models.Employee.EmployeeEntityModel CurrentEmployeeModel { get; set; }
        public List<CustomerOrder> ListOrder { get; set; }
        public List<CustomerOrderDetailEntityModel> ListOrderDetail { get; set; }
    }
}
