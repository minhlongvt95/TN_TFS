using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetDataSearchTopReVenueResult: BaseResult
    {
        public List<DataAccess.Models.Employee.EmployeeEntityModel> ListEmployee { get; set; }
        public DataAccess.Models.OrganizationEntityModel CurrentOrganization { get; set; }
        public List<DataAccess.Models.Customer.CustomerEntityModel> ListCustomer { get; set; }
        public List<DataAccess.Models.CategoryEntityModel> ListCustomerGroupCategory { get; set; }
        public List<DataAccess.Models.ProductCategory.ProductCategoryEntityModel> ListProductCategory { get; set; }
        public DataAccess.Models.Quote.InforExportExcelModel InforExportExcel { get; set; }
    }
}
