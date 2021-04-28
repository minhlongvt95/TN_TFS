using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Results.ProcurementRequest
{
    public class GetMasterDataSearchProcurementRequestResult : BaseResult
    {
        public List<Databases.Entities.Employee> Employees { get; set; }
        public List<CategoryEntityModel> ListStatus { get; set; }
        public List<ProductEntityModel> ListProduct { get; set; }
        public List<VendorEntityModel> ListVendor { get; set; }
        public List<Databases.Entities.ProcurementPlan> ListBudget { get; set; }
    }
}
