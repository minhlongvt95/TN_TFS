using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.OrderStatus;
using TN.TNM.BusinessLogic.Models.PurchaseOrderStatus;
using TN.TNM.BusinessLogic.Models.Vendor;
using TN.TNM.DataAccess.Models.ProcurementRequest;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.BusinessLogic.Messages.Responses.Vendor
{
    public class GetMasterDataSearchVendorOrderResponse : BaseResponse
    {
        public List<VendorModel> ListVendor { get; set; }
        public List<PurchaseOrderStatusModel> ListOrderStatus { get; set; }
        public List<EmployeeModel> ListEmployee { get; set; }
        public List<ProcurementRequestEntityModel> ListProcurementRequest { get; set; }
        public List<ProductEntityModel> ListProduct { get; set; }
        public DataAccess.Models.CompanyConfigEntityModel CompanyConfig { get; set; }
    }
}
