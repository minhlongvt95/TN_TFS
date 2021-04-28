using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetEmployeeListByOrganizationIdResult : BaseResult
    {
        public List<dynamic> employeeList { get; set; }
        public List<dynamic> lstResult { get; set; }
        public int? levelMaxProductCategory { get; set; }
        public List<CustomerOrder> lstOrderInventoryDelivery { get; set; }
        public List<CustomerOrder> lstOrderBill { get; set; }
        public List<dynamic> statusOrderList { get; set; }
        public List<dynamic> monthOrderList { get; set; }
    }
}
