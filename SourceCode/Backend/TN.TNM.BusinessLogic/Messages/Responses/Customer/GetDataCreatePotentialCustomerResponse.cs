using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.GeographicalArea;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Customer
{
    public class GetDataCreatePotentialCustomerResponse: BaseResponse
    {
        public List<CategoryModel> ListInvestFund { get; set; }
        public List<Models.Employee.EmployeeModel> ListEmployeeModel { get; set; }
        public List<GeographicalAreaEntityModel> ListArea { get; set; }
        public List<ProvinceEntityModel> ListProvinceEntityModel { get; set; }
        public List<DistrictEntityModel> ListDistrictEntityModel { get; set; }
        public List<WardEntityModel> ListWardEntityModel { get; set; }
        public List<CategoryModel> ListGroupCustomer { get; set; }
        public List<CustomerEntityModel> ListCustomer { get; set; }
        public List<EmployeeEntityModel> ListEmployeeTakeCare { get; set; }
    }
}
