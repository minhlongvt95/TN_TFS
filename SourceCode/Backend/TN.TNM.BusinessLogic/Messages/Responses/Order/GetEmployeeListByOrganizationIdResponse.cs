using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Order;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class GetEmployeeListByOrganizationIdResponse : BaseResponse
    {
        public List<dynamic> employeeList { get; set; }
        public List<dynamic> lstResult { get; set; }
        public int? levelMaxProductCategory { get; set; }
        public List<CustomerOrderModel> lstOrderInventoryDelivery { get; set; }
        public List<CustomerOrderModel> lstOrderBill { get; set; }
        public List<dynamic> statusOrderList { get; set; }
        public List<dynamic> monthOrderList { get; set; }
    }
}
