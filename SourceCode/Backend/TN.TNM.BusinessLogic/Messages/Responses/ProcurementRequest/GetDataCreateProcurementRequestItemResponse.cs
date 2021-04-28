using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.BusinessLogic.Messages.Responses.ProcurementRequest
{
    public class GetDataCreateProcurementRequestItemResponse: BaseResponse
    {
        public List<DataAccess.Models.Vendor.VendorEntityModel> ListVendor { get; set; }
        public List<DataAccess.Models.Product.ProductEntityModel> ListProduct { get; set; }
        public List<CategoryEntityModel> ListMoneyUnit { get; set; }
        public List<Models.ProcurementPlan.ProcurementPlanModel> ListProcurementPlan { get; set; }
    }
}
