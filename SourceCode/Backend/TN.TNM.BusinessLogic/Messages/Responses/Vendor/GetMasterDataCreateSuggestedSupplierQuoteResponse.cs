using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.ProcurementRequest;
using TN.TNM.BusinessLogic.Models.Product;
using TN.TNM.BusinessLogic.Models.Vendor;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.BusinessLogic.Messages.Responses.Vendor
{
    public class GetMasterDataCreateSuggestedSupplierQuoteResponse : BaseResponse
    {
        public List<VendorModel> ListVendor { get; set; }
        public List<ProductModel> ListProduct { get; set; }
        public List<ProcurementRequestModel> ListProcurementRequest { get; set; }
        public List<ProcurementRequestItemModel> ListProcurementRequestItem { get; set; }
        public List<EmployeeModel> ListEmployee { get; set; }
        public List<CategoryModel> ListStatus { get; set; }
        public List<ProductVendorMappingModel> ListProductVendorMapping { get; set; }

        // get suggested supplier quote request
        public SuggestedSupplierQuotesModel SuggestedSupplierQuotes { get; set; }
        public InforExportExcelModel InforExportExcel { get; set; }
    }
}
