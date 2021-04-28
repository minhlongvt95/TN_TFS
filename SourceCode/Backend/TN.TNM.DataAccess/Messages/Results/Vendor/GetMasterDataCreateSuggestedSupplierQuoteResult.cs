using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.ProcurementRequest;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.Quote;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Results.Vendor
{
    public class GetMasterDataCreateSuggestedSupplierQuoteResult : BaseResult
    {
        public List<VendorEntityModel> ListVendor { get; set; }
        public List<ProductEntityModel> ListProduct { get; set; }
        public List<ProcurementRequestEntityModel> ListProcurementRequest { get; set; }
        public List<ProcurementRequestItemEntityModel> ListProcurementRequestItem { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<CategoryEntityModel> ListStatus { get; set; }
        public List<ProductVendorMappingEntityModel> ListProductVendorMapping { get; set;}

        // Get suggested supplier quote quest
        public SuggestedSupplierQuotesEntityModel SuggestedSupplierQuotes { get; set; }
        public InforExportExcelModel InforExportExcel { get; set; }
    }
}
