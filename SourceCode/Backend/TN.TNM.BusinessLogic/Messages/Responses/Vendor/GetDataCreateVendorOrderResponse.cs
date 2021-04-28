using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.PurchaseOrderStatus;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Contract;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.BusinessLogic.Messages.Responses.Vendor
{
    public class GetDataCreateVendorOrderResponse: BaseResponse
    {
        public List<Models.Category.CategoryModel> ListPaymentMethod { get; set; }
        public List<PurchaseOrderStatusModel> ListOrderStatus { get; set; }
        public List<DataAccess.Models.Employee.EmployeeEntityModel> ListEmployeeModel { get; set; }
        public List<Models.BankAccount.BankAccountModel> ListBankAccount { get; set; }
        public List<DataAccess.Models.Vendor.VendorCreateOrderEntityModel> VendorCreateOrderModel { get; set; }
        public List<DataAccess.Models.ProcurementRequest.ProcurementRequestEntityModel> ListProcurementRequest { get; set; }
        //public List<ContractEntityModel> ListContract { get; set; }
        public List<WareHouseEntityModel> ListWareHouse { get; set; }
        public List<CategoryEntityModel> ListVendorGroup { get; set; }
    }
}
