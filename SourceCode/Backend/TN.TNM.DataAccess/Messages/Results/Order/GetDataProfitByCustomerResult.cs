using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetDataProfitByCustomerResult: BaseResult
    {
        public List<DataAccess.Models.Customer.CustomerEntityModel> ListCustomer { get; set; }
        public List<DataAccess.Models.CategoryEntityModel> ListCustomerGroupCategory { get; set; }
        public List<DataAccess.Models.ProductCategory.ProductCategoryEntityModel> ListProductCategory { get; set; }
        public List<DataAccess.Models.Contract.ContractEntityModel> ListContract { get; set; }
        public DataAccess.Models.Quote.InforExportExcelModel InforExportExcel { get; set; }
    }
}
