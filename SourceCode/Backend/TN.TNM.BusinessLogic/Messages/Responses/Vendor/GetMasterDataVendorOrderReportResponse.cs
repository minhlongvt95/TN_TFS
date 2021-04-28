using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.OrderStatus;
using TN.TNM.BusinessLogic.Models.PurchaseOrderStatus;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.ProcurementRequest;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Responses.Vendor
{
    public class GetMasterDataVendorOrderReportResponse : BaseResponse
    {
        public List<VendorEntityModel> ListVendor { get; set; }
        public List<PurchaseOrderStatusModel> ListStatus { get; set; }
        public List<ProcurementRequestEntityModel> ListProcurementRequest { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
    }
}
