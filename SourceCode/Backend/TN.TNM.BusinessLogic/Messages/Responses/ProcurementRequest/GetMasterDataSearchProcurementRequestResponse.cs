using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.ProcurementPlan;
using TN.TNM.BusinessLogic.Models.Product;
using TN.TNM.BusinessLogic.Models.Vendor;

namespace TN.TNM.BusinessLogic.Messages.Responses.ProcurementRequest
{
    public class GetMasterDataSearchProcurementRequestResponse : BaseResponse
    {
        public List<EmployeeModel> ListEmployee { get; set; }
        public List<CategoryModel> ListStatus { get; set; }
        public List<ProductModel> ListProduct { get; set; }
        public List<ProcurementPlanModel> ListBudget { get; set; }
        public List<VendorModel> ListVendor { get; set; }
    }
}
