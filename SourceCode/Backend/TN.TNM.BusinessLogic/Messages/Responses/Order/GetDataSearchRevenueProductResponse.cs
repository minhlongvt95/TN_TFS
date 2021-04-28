using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.BusinessLogic.Messages.Responses.Order
{
    public class GetDataSearchRevenueProductResponse: BaseResponse
    {
        public List<DataAccess.Models.ProductCategory.ProductCategoryEntityModel> ListProductCategory { get; set; }
        public List<DataAccess.Models.Employee.EmployeeEntityModel> ListEmployee { get; set; }
        public List<DataAccess.Models.WareHouse.WareHouseEntityModel> ListWarehouse { get; set; }
        public List<DataAccess.Models.CategoryEntityModel> ListCustomerGroupCategory { get; set; }
        public List<DataAccess.Models.Customer.CustomerEntityModel> ListCustomer { get; set; }
        public DataAccess.Models.Quote.InforExportExcelModel InforExportExcel { get; set; }

    }
}
